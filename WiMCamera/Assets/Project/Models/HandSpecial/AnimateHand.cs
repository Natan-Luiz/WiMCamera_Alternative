using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimateHand : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private AnimateHand related;
    ActionBasedController xc;

    private void Start()
    {
        animator = GetComponent<Animator>();
        xc = transform.parent.GetComponent<ActionBasedController>();
    }


    public void HandAnimations()
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (xc == null)
        {
            xc = transform.parent.GetComponent<ActionBasedController>();
        }
        else
        {
            animator.SetFloat("Trigger", xc.activateActionValue.action.ReadValue<float>());
            animator.SetFloat("Grip", xc.selectActionValue.action.ReadValue<float>());
            //animator.SetFloat("Thumb", xc.);

        }     
    }

    void Animate()
    {
        HandAnimations();
        if (related != null)
        {
            related.HandAnimations();
        }
        else
        {
            related = GetComponentInParent<PortalTraveller>().graphicsClone?.GetComponent<AnimateHand>();
        }       
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }
}
