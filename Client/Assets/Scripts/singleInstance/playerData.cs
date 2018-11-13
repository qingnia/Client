using UnityEngine;
using UnityEditor;

public class PlayerData : SingleInstance<PlayerData>
{
    public static SingleNet netInstance;
    private PlayerData() { }

    public int RoleID { get; set; }
    public string Name { get; set; }
    public int Status { get; set; }

    public void Init(PublicInfo pinfo)
    {
        this.Name = pinfo.name;
        this.Status = pinfo.status;
    }
}