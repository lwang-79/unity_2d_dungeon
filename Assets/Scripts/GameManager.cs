using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            return;
        }

        // PlayerPrefs.DeleteAll();

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    // References
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public RectTransform hitpointBar;
    public Animator deathMenuAnim;
    public GameObject hud;
    public GameObject menu;

    // Logic
    public int pesos;
    public int experience;

    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    // Upgrade weapon
    public bool TryUpgradeWeapon()
    {
        // is the weapon max level?
        if (weaponPrices.Count <= weapon.weaponLevel)
        {
            return false;
        }

        if (pesos >= weaponPrices[weapon.weaponLevel])
        {
            pesos -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }

        return false;
    }

    // Hitpoint Bar
    public void OnHitpointChange()
    {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    // Experience System
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while(experience >= add)
        {
            add += xpTable[r];

            if (++r == xpTable.Count)
                return r;
        }

        return r;
    }

    public int GetXpToLevel(int level)
    {
        int l = 0;
        int xp = 0;

        while (l < level)
        {
            xp += xpTable[l];
            l++;
        }

        return xp;
    }

    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if (currLevel < GetCurrentLevel())
            OnLevelUp();
    }

    public void OnLevelUp()
    {
        player.OnLevelUp();
        OnHitpointChange();
    }

    // On Scene Loaded
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    // Death Menu and Respawn
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        instance.experience = 0;
        instance.weapon.SetWeaponLevel(0);
        SceneManager.LoadScene("Main");
        player.Respawn();
    }

    // Save state
    public void SaveSate()
    {
        string s = "";

        s += player.skinId.ToString() + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();
        
        PlayerPrefs.SetString("SaveState", s);
    }

    public void LoadState(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;


        if (PlayerPrefs.HasKey("SaveState"))
        {
            string[] data = PlayerPrefs.GetString("SaveState").Split('|');

            player.SwapSprite(int.Parse(data[0]));

            pesos = int.Parse(data[1]);

            experience = int.Parse(data[2]);
            if (GetCurrentLevel() != 1)
                player.SetLevel(GetCurrentLevel());

            weapon.SetWeaponLevel(int.Parse(data[3]));
        }
    }
}
