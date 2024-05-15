import SwiftUI

struct ContentView: View {
    @State private var status = DarkDeedsStatusResponse(header: "Loading...", items: [])
    @State private var loading = false
    
    var body: some View {
        ScrollView {
            VStack(alignment: .leading) {
                Text(status.header)
                    .font(.headline)
                    .padding(.bottom)
                Spacer()
                ForEach(status.items, id: \.self) { item in
                    Text(item.item)
                        .fontWeight(item.isSupport ? .ultraLight : .regular)
                        .lineLimit(1)
                }
                Spacer()
                Button("Refresh", systemImage: "arrow.clockwise", action: fetchStatusFromBackend)
                    .labelStyle(.iconOnly)
                    .tint(.blue)
                    .buttonStyle(.borderedProminent)
                    .padding(.top)
                    .disabled(loading)
            }
//            .background(.red)
            .background(.black)
            .frame(minHeight: 165)
        }
//        .background(.green)
        .padding(.horizontal)
        .padding(.top, -15)
        .onAppear {
            fetchStatusFromBackend()
        }
    }
    
    func fetchStatusFromBackend() {
        guard let url = URL(string: "https://dark-deeds.com/api/mobile/watch/f54d3975-d313-475d-a171-e85b05acf5e7/app") else {
            print("Invalid URL")
            return
        }
        
        DispatchQueue.main.async {
            self.loading = true
        }
        
        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        
        URLSession.shared.dataTask(with: request) { data, response, error in
            if let error = error {
                print("Error: \(error)")
                return
            }
            
            guard let data = data else {
                print("No data received")
                return
            }
            
            do {
                let status = try JSONDecoder().decode(DarkDeedsStatusResponse.self, from: data)
                DispatchQueue.main.async {
                    self.status = status
                    self.loading = false
                }
            } catch {
                print("Error decoding JSON: \(error)")
            }
        }.resume()
    }

}

struct DarkDeedsStatusResponse: Decodable {
    var header: String
    var items: [DarkDeedsStatusItem]
}

struct DarkDeedsStatusItem: Decodable, Hashable {
    var item: String
    var isSupport: Bool
}

#Preview {
    ContentView()
}
