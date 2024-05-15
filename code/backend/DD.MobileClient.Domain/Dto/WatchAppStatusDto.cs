namespace DD.MobileClient.Domain.Dto;

public record WatchAppStatusDto(string Header, IReadOnlyList<WatchAppStatusItemDto> Items);
