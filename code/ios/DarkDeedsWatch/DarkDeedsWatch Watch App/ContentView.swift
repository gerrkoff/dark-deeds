import SwiftUI

struct ContentView: View {
    @State private var status = DarkDeedsStatusResponse(header: "Loading...", main: "", support: "")
    var version = "v1"
    
    var body: some View {
        VStack {
//            Image(systemName: "globe")
//                .imageScale(.large)
//                .foregroundStyle(.tint)
//            Text("Hello, world!")
            
            Text(status.header)
                .font(.callout)
                .padding(.bottom)
            Text(status.main)
                .font(.headline)
                .padding(.bottom)
            Text(status.support)
                .font(.callout)
                .fontWeight(.ultraLight)
                
            Text(version)
                .font(.caption2)
                .fontWeight(.ultraLight)
                .padding(.top, 4.0)
        }
        .padding()
        .onAppear {
            fetchStatusFromBackend()
        }
    }
    
    func fetchStatusFromBackend() {
        guard let url = URL(string: "https://dark-deeds.com/api/mobile/watch/f54d3975-d313-475d-a171-e85b05acf5e7") else {
            print("Invalid URL")
            return
        }
        
        // Create a URLRequest
        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        
        // Send the request
        URLSession.shared.dataTask(with: request) { data, response, error in
            // Check for errors
            if let error = error {
                print("Error: \(error)")
                return
            }
            
            // Check if response contains data
            guard let data = data else {
                print("No data received")
                return
            }
            
            // Parse JSON response
            do {
                let status = try JSONDecoder().decode(DarkDeedsStatusResponse.self, from: data)
                DispatchQueue.main.async {
                    // Update the UI with the message from the backend
                    self.status = status
                }
            } catch {
                print("Error decoding JSON: \(error)")
            }
        }.resume()
    }

}

struct DarkDeedsStatusResponse: Decodable {
    var header: String
    var main: String
    var support: String
}

#Preview {
    ContentView()
}
