using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PomDataHandler
{
    /// <summary>
    /// Returns all the Player Positions.
    /// </summary>
    public static Vector3[] GetAllPlayerPos(int player, PommermanData data)
    {
        ValidatePlayerID(player);
        int stateCount = data.state.Length;

        // Check when player dies
        int step = 0;
        while (step < stateCount && data.state[step].agents[player].is_alive)
        {
            step++;
        }

        stateCount = step;
        Vector3[] positions = new Vector3[stateCount];
        for (int i = 0; i < stateCount; i++)
        {
            int x = data.state[i].agents[player].position[0];
            int z = data.state[i].agents[player].position[1];
            positions[i] = new Vector3(x, i, z);
        }

        return positions;
    }

    /// <summary>
    /// Returns the position of a player at a specific step.
    /// </summary>
    public static Vector3 GetPlayerPosAt(int player, int step, PommermanData data)
    {
        ValidatePlayerID(player);

        if (step > data.state.Length) return Vector3.zero;
        if (IsPlayerDead(player, step, data)) return Vector3.zero;

        int x = data.state[step].agents[player].position[0];
        int z = data.state[step].agents[player].position[1];

        return new Vector3(x, step, z); ;
    }

    /// <summary>
    /// Retrieves the positions of a specified player from a starting step to an ending step,
    /// ensuring the player ID and step values are within valid bounds. 
    /// The positions are returned as an array of Vector3, with each vector representing 
    /// the player's position at a specific step.
    /// </summary>
    public static Vector3[] GetAllPlayerPosFromTo(int player, int fromStep, int toStep, PommermanData data)
    {
        ValidatePlayerID(player);

        int maxSteps = data.state.Length;

        if (fromStep < 0 || toStep < 0 || toStep < fromStep || fromStep > maxSteps || toStep > maxSteps)
        {
            Debug.LogError("fromStep or toStep value is invalid. Set to 0 and 1");
            fromStep = 0;
            toStep = 1;
        }

        List<Vector3> positions = new List<Vector3>();
        int step = fromStep;
        while (step != toStep + 1 && data.state[step].agents[player].is_alive)
        {
            int x = data.state[step].agents[player].position[0];
            int z = data.state[step].agents[player].position[1];
            positions.Add(new Vector3(x, step, z));
            step++;
        }

        return positions.ToArray();
    }

    /// <summary>
    /// Return if a player is dead at a given time step.
    /// </summary>
    public static bool IsPlayerDead(int player, int step, PommermanData data)
    {
        ValidatePlayerID(player);

        if (step < 0 || step >= data.state.Length)
        {
            Debug.LogError("step is out of bounce. Return FALSE");
            return false;
        }
        return !data.state[step].agents[player].is_alive;
    }

    /// <summary>
    /// Return the first step a player is on a specific position.
    /// </summary>
    private static int GetFirstStepPlayerIsOnTile(int playerID, int from, int to, Vector2 position, PommermanData data)
    {
        for (int step = from; step <= to; step++)
        {
            Vector3 playerPosition = GetPlayerPosAt(playerID, step, data);
            if (playerPosition.x == position.x && playerPosition.z == position.y)
            {
                return step;
            }
        }
        return int.MaxValue;
    }

    /// <summary>
    /// Check for each item when a player was collecting it.
    /// </summary>
    public static Dictionary<Vector3, int> GetPickedUpItems(int from, int to, PommermanData data)
    {
        // Get all items at the starting position
        int fixedFrom = from - 5;
        if (fixedFrom < 0) fixedFrom = 0;
        List<Item> items = data.state[fixedFrom].items.ToList();
        Dictionary<Vector3, int> collectedPowerUps = new Dictionary<Vector3, int>();

        foreach (Item item in items)
        {
            Vector2 itemPosition = new Vector2(item.position[0], item.position[1]);
            int firstContact = int.MaxValue;

            for (int player = 0; player <= 3; player++)
            {
                int contact = GetFirstStepPlayerIsOnTile(player, from, to, itemPosition, data);
                if (contact < firstContact) firstContact = contact;
            }

            if (firstContact >= to) continue;

            Vector3 itemCollectedPosition = new Vector3(item.position[0], firstContact, item.position[1]);

            // Create new List if the given type is new
            if (!collectedPowerUps.ContainsKey(itemCollectedPosition))
                collectedPowerUps.Add(itemCollectedPosition, item.type);
        }

        return collectedPowerUps;
    }

    /// <summary>
    /// Get the board states between 'from' and 'to' steps.
    /// </summary>
    public static int[][][] GetBoardStatesFromTo(int from, int to, PommermanData data)
    {
        if (from > to || from < 0 || to >= data.state.Length) return null;

        int stepCount = to - from + 1;
        int[][][] boardStates = new int[stepCount][][];

        for (int i = 0, index = from; i < stepCount; i++, index++)
        {
            boardStates[i] = data.state[index].board;
        }

        return boardStates;
    }

    /// <summary>
    /// Return the board size.
    /// </summary>
    public static int GetBoardSize(PommermanData data)
    {
        return data.state[0].board_size;
    }

    /// <summary>
    /// Return the number of states a game has.
    /// </summary>
    public static int GetGameLength(PommermanData data)
    {
        return data.state.Length;
    }

    /// <summary>
    // Returns the flames in a dictionary, sorted by remaining life
    /// </summary>
    public static Dictionary<int, List<Vector3>> GetAllFlamesPosFromTo(int from, int to, PommermanData data)
    {
        if (to > data.state.Length) to = data.state.Length;
        if (from < 0) from = 0;

        Dictionary<int, List<Vector3>> flames = new Dictionary<int, List<Vector3>>();

        for (int i = from; i <= to; i++)
        {
            if (data.state[i].flames.Length <= 0) continue;

            // Iterate over each flame at the current time step
            for (int j = 0; j < data.state[i].flames.Length; j++)
            {
                Flame curFlame = data.state[i].flames[j];

                // Create new List if the given life is new
                if (!flames.ContainsKey(curFlame.life))
                {
                    flames.Add(curFlame.life, new List<Vector3>());
                }

                Vector2 position = new Vector2(
                    data.state[i].flames[j].position[0],
                    data.state[i].flames[j].position[1]
                );

                flames[curFlame.life].Add(new Vector3(position.x, i, position.y));
            }
        }

        return flames;
    }

    /// <summary>
    /// Returns the position of every bomb, including the same bomb for multiple steps from and to a specific step.
    /// </summary>
    public static Dictionary<int, List<Vector3>> GetAllPlayerBombsFromTo(int player, int from, int to, PommermanData data)
    {
        ValidatePlayerID(player);

        if (to > data.state.Length) to = data.state.Length;
        if (from < 0) from = 0;

        Dictionary<int, List<Vector3>> bombs = new Dictionary<int, List<Vector3>>();

        for (int i = from; i <= to; i++)
        {
            if (data.state[i].bombs.Length <= 0) continue;

            // Iterate over each bomb at the current time step
            for (int j = 0; j < data.state[i].bombs.Length; j++)
            {
                Bomb curBomb = data.state[i].bombs[j];
                // Skip if bomb was not layed by given player
                if (curBomb.bomber_id != player) continue;

                // Create new List if the given Bomb ID is new
                if (!bombs.ContainsKey(curBomb.bombId))
                {
                    bombs.Add(curBomb.bombId, new List<Vector3>());
                }

                Vector2 position = new Vector2(
                    data.state[i].bombs[j].position[0],
                    data.state[i].bombs[j].position[1]
                );

                bombs[curBomb.bombId].Add(new Vector3(position.x, i, position.y));
            }
        }
        return bombs;
    }

    /// <summary>
    /// Calculate the number of bombs placed in each cell from 'from' to 'to' steps.
    /// </summary>
    public static int[,] CalculateBombPlacementsPerCell(int from, int to, PommermanData data)
    {
        int boardSize = data.state[0].board_size;
        int[,] resMatrix = new int[boardSize, boardSize];

        for (int step = from; step < to; step++)
        {
            Bomb[] bomArray = data.state[step].bombs;

            if (bomArray.Length > 0)
            {
                foreach (Bomb bomb in bomArray)
                {
                    int x = bomb.position[0];
                    int y = bomb.position[1];
                    resMatrix[x, y] = resMatrix[x, y] + 1;
                }
            }
        }

        return resMatrix;
    }

    /// <summary>
    /// Returns the Game ID
    /// </summary>
    public static int GetGameId(PommermanData data)
    {
        return data.game_id;
    }

    // Helper 
    private static void ValidatePlayerID(int player)
    {
        if (player < 0 || player > 3)
        {
            Debug.LogError($"playerID {player} is out of bounce. Set to 0");
            player = 0;
        }

        return;
    }
}
