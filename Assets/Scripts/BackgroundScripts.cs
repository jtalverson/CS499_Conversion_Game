using UnityEngine;

public class BackgroundScripts : MonoBehaviour
{
    // Start is called before the first frame update
    // this script should be placed on the generic background canvas
    public RectTransform BaseColor;
    public RectTransform SquaresPattern;


    public Color backgroundBaseColor;
    public Color backgroundBaseNoTimeColor;

    public Color backgroundSquareColor;
    public Color backgroundSquareNoTimeColor;

    public Timer timerController;

    void Start()
    {

        // scale the base color sprite to match the canvas
        Vector2 canvasSize = this.GetComponent<RectTransform>().sizeDelta;
        Vector3 baseColorScale = new Vector3(canvasSize[0], canvasSize[1], 1);
        BaseColor.localScale = baseColorScale;
        // scale the square pattern to match the canvas
        Vector2 patternSize = SquaresPattern.sizeDelta;
        // scale calculation should always fit the pattern to the HEIGHT of the canvas
        Vector3 patternScale = new Vector3(canvasSize[1] / patternSize[1], canvasSize[1] / patternSize[1], canvasSize[1] / patternSize[1]);
        SquaresPattern.localScale = patternScale;
    }

    private void Update()
    {
        if (timerController.timerIsRunning)
        {
            if (timerController.timeRemaining > 0)
            {
                float timeRemainingNormalized = 1 - (timerController.timeRemaining / timerController.maxTime);
                BaseColor.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(backgroundBaseColor, backgroundBaseNoTimeColor, timeRemainingNormalized);
                SquaresPattern.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(backgroundSquareColor, backgroundSquareNoTimeColor, timeRemainingNormalized);
            }
        }
    }

    public void ResetBackground()
    {
        BaseColor.gameObject.GetComponent<SpriteRenderer>().color = backgroundBaseColor;
        SquaresPattern.gameObject.GetComponent<SpriteRenderer>().color = backgroundSquareColor;
    }

}
