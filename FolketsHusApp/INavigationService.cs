namespace FolketsHusApp;

public interface INavigationService {
    Task GoToAsync(string route);
}
