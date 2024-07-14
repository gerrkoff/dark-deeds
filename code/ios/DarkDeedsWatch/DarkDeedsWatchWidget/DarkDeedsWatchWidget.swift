import WidgetKit
import SwiftUI

@main
struct DarkDeedsStatusWidget: Widget {
    var body: some WidgetConfiguration {
        StaticConfiguration(
            kind: "com.dark-deeds.dark-deeds-status",
            provider: DarkDeedsStatusProvider()
        ) { entry in
            DarkDeedsStatusView(entry: entry)
                .containerBackground(.fill.tertiary, for: .widget)
        }
        .configurationDisplayName("Dark Deeds Status")
        .description("Shows an overview of your Dark Deeds status")
        .supportedFamilies([.accessoryRectangular])
    }
}

struct DarkDeedsStatusEntry: TimelineEntry {
    var date: Date
    var header: String
    var main: String
    var support: String
}

struct DarkDeedsStatusResponse: Decodable {
    var header: String
    var main: String
    var support: String
}

struct DarkDeedsStatusProvider: TimelineProvider {
    func placeholder(in context: Context) -> DarkDeedsStatusEntry {
        return DarkDeedsStatusEntry(date: Date(), header: "-2 (-3) remaining", main: "Do something", support: "Do this also")
    }

    func getSnapshot(in context: Context, completion: @escaping (DarkDeedsStatusEntry) -> Void) {
        completion(DarkDeedsStatusEntry(date: Date(), header: "-1 (-2) remaining", main: "Do something", support: "Do this also"))
    }

    func getTimeline(in context: Context, completion: @escaping (Timeline<DarkDeedsStatusEntry>) -> Void) {
        fetchStatusFromBackend(completion: completion)
    }

    func getTimelineEntry(status: DarkDeedsStatusResponse) -> Timeline<DarkDeedsStatusEntry> {
        // Create a timeline entry for "now."
        let date = Date()
        let entry = DarkDeedsStatusEntry(
            date: date,
            header: status.header,
            main: status.main,
            support: status.support
        )

        // Create a date that's 15 minutes in the future.
        let nextUpdateDate = Calendar.current.date(byAdding: .minute, value: 15, to: date)!

        // Create the timeline with the entry and a reload policy with the date
        // for the next update.
        let timeline = Timeline(
            entries:[entry],
            policy: .after(nextUpdateDate)
        )

        return timeline
    }

    func fetchStatusFromBackend(completion: @escaping (Timeline<DarkDeedsStatusEntry>) -> Void) {
        print("DBG: fetching")
        guard let url = URL(string: "https://dark-deeds.com/api/mobile/watch/f54d3975-d313-475d-a171-e85b05acf5e7/widget") else {
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
                let timelineEntry = getTimelineEntry(status: status)
                print("DBG: updating")
                completion(timelineEntry)
            } catch {
                print("Error decoding JSON: \(error)")
            }
        }.resume()
    }
}

struct DarkDeedsStatusView : View {
    @Environment(\.widgetFamily) var family: WidgetFamily
    var entry: DarkDeedsStatusProvider.Entry

    @ViewBuilder
    var body: some View {
        switch family {
        case .accessoryRectangular: DarkDeedsSummary(entry: entry)
        default: DarkDeedsNotAvailable()
        }
    }
}

struct DarkDeedsSummary : View {
    var entry: DarkDeedsStatusProvider.Entry

    var body: some View {
        VStack(alignment: .leading) {
            Text(entry.header)
                .font(.callout)

            if (!entry.main.isEmpty) {
                Text(entry.main)
                    .font(.headline)
            }

            if (!entry.support.isEmpty) {
                Text(entry.support)
                    .font(.callout)
                    .fontWeight(.ultraLight)
            }
        }
        .frame(maxWidth: .infinity, alignment: .leading)
    }
}

struct DarkDeedsNotAvailable : View {
    var body: some View {
        VStack {
            Text("GameDetailsNotAvailable")
        }
    }
}

#Preview(as: .accessoryRectangular) {
    DarkDeedsStatusWidget()
} timeline: {
    DarkDeedsStatusEntry(date: .now, header: "1 remaining", main: "Do something", support: "Do this also")
    DarkDeedsStatusEntry(date: .now, header: "1 remaining", main: "", support: "Do this also")
    DarkDeedsStatusEntry(date: .now, header: "1 remaining", main: "Do something", support: "")
    DarkDeedsStatusEntry(date: .now, header: "1 remaining", main: "", support: "")
}
