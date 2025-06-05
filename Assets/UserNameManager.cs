using UnityEngine;

public class UserNameManager : MonoBehaviour
{
    public static UserNameManager Instance { get; private set; }

    private string userName = "Anonymous";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SetUserName(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            userName = newName;
        }
        else
        {
            userName = "N/A";
        }
    }

    public string GetUserName()
    {
        return userName;
    }
}
