namespace Mestra.Interfaces;

/// <summary>
///     Coordinates sending requests and publishing notifications through the Mestra pipeline.
/// </summary>
public interface IMediator : ISender, IPublisher;