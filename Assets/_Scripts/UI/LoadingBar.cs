using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Umbr.EF.UI
{
    public class LoadingBar : MonoBehaviour
    {
        Image image;
        void Start()
        {
            image = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            image.fillAmount = Loader.GetLoadingProgress();
        }
    }
}
