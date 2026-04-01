using UnityEngine;

public class Beehaviours : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WhatIsBeezDoing(BeezakaManager.WhatShouldBeezDo whatShouldBeezDo)
    {
        switch (whatShouldBeezDo)
        {
            case BeezakaManager.WhatShouldBeezDo.InHell:
                Debug.Log("she certainly is in hell");
                break;
            case BeezakaManager.WhatShouldBeezDo.Spawning:
                Debug.Log("you're gonna spawn alright");
                break;
            case BeezakaManager.WhatShouldBeezDo.Creeping:
                Debug.Log("you're gonna creep alright");
                break;
            case BeezakaManager.WhatShouldBeezDo.Stalking:
                Debug.Log("you're gonna stalk alright");
                break;

        }
    }
}
