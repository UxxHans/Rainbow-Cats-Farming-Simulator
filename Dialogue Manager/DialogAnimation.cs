using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script gives the UI animation of typing
/// </summary>
public class DialogAnimation : MonoBehaviour
{
    //String content
    [TextArea]
    public List<string> content;
    //Show speed
    public float speed;
    //The sound of each letter
    public AudioClip sound;
    //End button
    public Button endbutton;

    //Private variables
    private int c_ContentIndex;
    private Text c_TextComponent;
    private int c_LetterCount;
    private string c_Content;
    private AudioSource c_AudioSource;

    /// <summary>
    /// Show one letter, this will be called only once to start, the loop will do its thing
    /// Instead of invokeRepeating, I think this has more stability and controls.
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartAnimation(int contentIndex)
    {
        //Clean the Content
        c_Content = "";

        //For every letter in the content
        for(int i = 0; i < c_LetterCount; i++)
        {
            //Set the speed as normal speed
            float c_Speed = speed;
            //Set current letter using index
            char c_letter = content[contentIndex][i];
            //Add letter to the text to display
            c_Content += c_letter;
            //Refresh the text
            c_TextComponent.text = c_Content;

            //If the letter is comma, dot or space
            switch (c_letter)
            {
                //Double the wait time if letter is space
                case ' ':
                    c_Speed *= 2;
                    break;
                //Triple the wait time if letter is comma
                case ',':
                    c_Speed *= 3;
                    break;
                //Quad the wait time if letter is dot
                case '.':
                    c_Speed *= 4;
                    break;
            }

            //Stop the overlap audio to prevent messy hearing
            if (c_AudioSource.isPlaying) c_AudioSource.Stop();
            //Play sound once
            c_AudioSource.PlayOneShot(sound);

            //Wait the interval
            yield return new WaitForSeconds(c_Speed);
        }
    }

    /// <summary>
    /// Initialize each time its enabled
    /// </summary>
    public void OnEnable()
    {
        //Hide the end button
        endbutton.gameObject.SetActive(false);

        //Set content index
        c_ContentIndex = 0;
        //Get text component
        c_TextComponent = GetComponent<Text>();
        //Get the count of letters
        c_LetterCount = content[c_ContentIndex].Length;
        //Get the audio component
        c_AudioSource = FindObjectOfType<AudioSource>();

        //Start Animation
        StartCoroutine(StartAnimation(c_ContentIndex));
    }

    /// <summary>
    /// Display the content with given index
    /// </summary>
    /// <param name="contentIndex">Index</param>
    public bool DisplayContent(int contentIndex)
    {
        //Set end button
        if (contentIndex >= content.Count || contentIndex < 0)
            return false;
        if (contentIndex >= content.Count-1)
            endbutton.gameObject.SetActive(true);
        else
            endbutton.gameObject.SetActive(false);

        //Stop all animation
        StopAllCoroutines();

        //Set content index
        c_ContentIndex = contentIndex;
        //Get text component
        c_TextComponent = GetComponent<Text>();
        //Get the count of letters
        c_LetterCount = content[contentIndex].Length;
        //Get the audio component
        c_AudioSource = FindObjectOfType<AudioSource>();

        //Start Animation
        StartCoroutine(StartAnimation(contentIndex));

        return true;
    }
    public void DisplayNextContent() => DisplayContent(c_ContentIndex+1);
    public void DisplayPreviousContent() => DisplayContent(c_ContentIndex-1);
}
