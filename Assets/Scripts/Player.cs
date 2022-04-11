using UnityEngine;

public class Player : Mover
{
    public int skinId = 0;
    private SpriteRenderer spriteRenderer;
    private bool isAlive = true;

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive)
            return;

        base.ReceiveDamage(dmg);
        GameManager.instance.OnHitpointChange();
    }

    protected override void Death()
    {
        isAlive = false;
        GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            UpdateMotor(new Vector3(x, y, 0));
        }
    }

    public void SwapSprite(int skinId)
    {
        this.skinId = skinId;
        spriteRenderer.sprite = GameManager.instance.playerSprites[skinId];
    }

    public void OnLevelUp()
    {
        maxHitpoint++;
        hitpoint = maxHitpoint;
        GameManager.instance.ShowText("Level Up", 25, Color.red, transform.position, Vector3.up * 40, 1);
    }

    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
        {
            maxHitpoint++;
            hitpoint = maxHitpoint;
        }
    }

    public void Heal(int healingAmount)
    {
        if (hitpoint < maxHitpoint)
        {
            hitpoint += healingAmount;
            GameManager.instance.ShowText($"+ {healingAmount} hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
            GameManager.instance.OnHitpointChange();
        }
    }

    public void Respawn()
    {
        Heal(maxHitpoint);
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
    }
}
