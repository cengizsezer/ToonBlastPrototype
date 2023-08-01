using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FlyingSprite : PoolObject
{
  

    public override void OnCreated()
    {
        OnDeactive();
    }
    public override void OnDeactive()
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    public override void OnSpawn()
    {
        gameObject.SetActive(true);
    }
}
