using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Character
{
    public string characterName;

    /// <summary>
    /// The root is the container for all images related to the character in the scene. The root object.
    /// </summary>
    [HideInInspector] public RectTransform root;
    
    public bool enabled {get{return root.gameObject.activeInHierarchy;} set {root.gameObject.SetActive (value);}}


    DialogueSystem dialogue;
    
    /// <summary>
    /// Make this character say something.
    /// </summary>
    /// <param name="speech">Speech.</param>
    public void Say(string speech, bool add = false)
    {
        if (!enabled)
            enabled = true;

        dialogue.Say (speech, characterName, add);
    }

    //Begin Transitioning Images\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	public Sprite GetSprite(int index = 0)
	{
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Characters/" + characterName);
		return sprites[index];
	}

    public Sprite GetSprite(string spriteName = "")
	{
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Characters/" + characterName);
		for(int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i].name == spriteName)
				return sprites[i];
		}
		return sprites.Length > 0 ? sprites[0] : null;
	}

	public void SetBody(int index)
	{
		renderers.bodyRenderer.sprite = GetSprite (index);
	}
	public void SetBody(Sprite sprite)
	{
		renderers.bodyRenderer.sprite = sprite;
	}
    public void SetBody(string spriteName)
	{
        renderers.bodyRenderer.sprite = GetSprite (spriteName);
	}

    //Transition Body
	bool isTransitioningBody {get{ return transitioningBody != null;}}
	Coroutine transitioningBody = null;

	public void TransitionBody(Sprite sprite, float speed, bool smooth)
	{
		StopTransitioningBody ();
		transitioningBody = CharacterManager.instance.StartCoroutine (TransitioningBody (sprite, speed, smooth));
	}

	void StopTransitioningBody()
	{
		if (isTransitioningBody)
			CharacterManager.instance.StopCoroutine (transitioningBody);
		transitioningBody = null;
	}

	public IEnumerator TransitioningBody(Sprite sprite, float speed, bool smooth)
	{
		for (int i = 0; i < renderers.allBodyRenderers.Count; i++) 
		{
			Image image = renderers.allBodyRenderers [i];
			if (image.sprite == sprite) 
			{
				renderers.bodyRenderer = image;
				break;
			}
		}

		if (renderers.bodyRenderer.sprite != sprite) 
		{
			Image image = GameObject.Instantiate (renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent).GetComponent<Image> ();
			renderers.allBodyRenderers.Add (image);
			renderers.bodyRenderer = image;
			image.color = GlobalF.SetAlpha (image.color, 0f);
			image.sprite = sprite;
		}

		while (GlobalF.TransitionImages (ref renderers.bodyRenderer, ref renderers.allBodyRenderers, speed, smooth))
			yield return new WaitForEndOfFrame ();

		StopTransitioningBody ();
	}

	//End Transition Images\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    public bool isVisibleInScene
    {
        get { return visibleInScene; }
    }
    bool visibleInScene = true;

	public void FadeOut(float speed = 3, bool smooth = false)
	{
		Sprite alphaSprite = Resources.Load<Sprite>("Images/AlphaOnly");

		lastBodySprite = renderers.bodyRenderer.sprite;

		TransitionBody(alphaSprite, speed, smooth);

        visibleInScene = false;
	}

	Sprite lastBodySprite, lastFacialSprite = null;
	public void FadeIn(float speed = 3, bool smooth = false)
	{
		if (lastBodySprite != null && lastFacialSprite != null)
		{
			TransitionBody(lastBodySprite, speed, smooth);

            if (enabled)
                visibleInScene = true;
        }
	}

    /// <summary>
    /// Create a new character.
    /// </summary>
    /// <param name=" _name">Name.</param>
    public Character (string _name, bool enableOnStart = true)
    {
        CharacterManager cm = CharacterManager.instance;
        // locate the character prefab.
        GameObject prefab = Resources.Load ("Characters/Character[" + _name + "]") as GameObject;
        // Spawn an instance of the prefab directly on the character panel.
        GameObject ob = GameObject.Instantiate (prefab, cm.characterPanel);

        root = ob.GetComponent<RectTransform> ();
        characterName = _name;

        //get the renderer(s)
        renderers.bodyRenderer = ob.transform.Find ("bodyLayer").GetComponent<Image> ();
        renderers.allBodyRenderers.Add (renderers.bodyRenderer);

        dialogue = DialogueSystem.instance;

        enabled = enableOnStart;
    }

    [System.Serializable]
    public class Renderers
    {
        //sprites use images.
        /// <summary>
        /// The body renderer for a multi layer character.
        /// </summary>
        public Image bodyRenderer;

        public List<Image> allBodyRenderers = new List<Image>();
    }

    public Renderers renderers = new Renderers();
}
