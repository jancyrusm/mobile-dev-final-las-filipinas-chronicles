using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for adding and maintaining characters in the scene.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    /// <summary>
    /// All characters must be attached to the character panel.
    /// </summary>
    public RectTransform characterPanel;

    /// <summary>
    /// A list of all characters currently in our scene.
    /// </summary>
    public List<Character> characters = new List<Character>();
    
    /// <sumary>
    /// Easy lookup for our characters.
    /// </summary>
    public Dictionary<string, int> characterDictionary = new Dictionary<string, int>();

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Try to get a character by the name provided from th character list.
    /// </summary>
    /// <param name="characterName">Character name,</param>
    public Character GetCharacter(string characterName, bool createCharacterIfDoesNotExist = true, bool enableCreatedCharacterOnStart = true)
    {
        //search our dictionary to find the character quickly if it is already in our scene.
        int index = -1;
        if (characterDictionary.TryGetValue (characterName, out index))
        {
            return characters [index];
        }
        else if (createCharacterIfDoesNotExist)
        {
            return CreateCharacter (characterName, enableCreatedCharacterOnStart);
        }

        return null;
    }

    /// <summary>
    /// Creates the character.
    /// </summary>
    /// <returns>The character.</returns>
    /// <param name="characterName">Character name.</param>
    public Character CreateCharacter(string characterName, bool enableOnStart = true)
    {
        Character newCharacter = new Character (characterName, enableOnStart);

        characterDictionary.Add (characterName, characters.Count);
        characters.Add (newCharacter);

        return newCharacter;
    }
}
