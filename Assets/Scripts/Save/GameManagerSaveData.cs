using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameManagerSaveData : Saveable<GameManagerSaveData>
{
    private int currentLevelIndex = 0;
    public int CurrentLevelIndex => currentLevelIndex;

    public void ProgressLevel()
    {
        currentLevelIndex++;
        Save();
    }

    public void ResetLevelIndex()
    {
        currentLevelIndex = 0;
        Save();
    }
}
