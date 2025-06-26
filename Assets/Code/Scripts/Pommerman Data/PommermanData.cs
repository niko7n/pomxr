[System.Serializable]
public class Agent
{
    public int agent_id;
    public bool is_alive;
    public int[] position;
    public int ammo;
    public int blast_strength;
    public bool can_kick;
}

[System.Serializable]
public class Bomb
{
    public int[] position;
    public int bomber_id;
    public int life;
    public int blast_strength;
    //public int moving_direction
    public int bombId;
}

[System.Serializable]
public class Flame
{
    public int[] position;
    public int life;
}

[System.Serializable]
public class Item
{
    public int[] position;
    public int type;
}

[System.Serializable]
public class State
{
    public Agent[] agents;
    public int[][] board;
    public int board_size;
    public int step_count;
    public Bomb[] bombs;
    public Flame[] flames;
    public Item[] items;
}

[System.Serializable]
public class Result
{
    public int id;
    public string name;
}

[System.Serializable]
public class PommermanData
{
    public Result result;
    public int game_id;
    public State[] state;
}
