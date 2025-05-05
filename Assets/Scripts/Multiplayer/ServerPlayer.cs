using Photon.Pun;

public class ServerPlayer : BasePlayer // Very much unfinished
{
    public int score;

    private void Start()
    {
        try { identity = PhotonNetwork.NickName; }
        catch { }

        if (FindAnyObjectByType<DeathmatchManager>() != null)
        {
            FindAnyObjectByType<DeathmatchManager>().GetComponent<DeathmatchManager>().players.Add(this);
        }
    }

    public virtual void AddScore(int amount)
    {
        score += amount;
    }

    public override void Die()
    {
        base.Die();
        AddScore(1); // to the killer idk who that was
    }
}