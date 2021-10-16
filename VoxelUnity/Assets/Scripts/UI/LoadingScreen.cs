using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class LoadingScreen: MonoBehaviour
	{
	    public Text StatusText;
	    public Image ProgressBarImage;

	    public void SetProgress(float progress)
	    {
		    ProgressBarImage.fillAmount = progress;
	    }
	}

}