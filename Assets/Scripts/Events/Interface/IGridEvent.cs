using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGridEvent
{
    public void OnStart<T>(GridController grid, List<T> effectedEntities) where T : IGridEntity;
    public void OnComplate();


}
