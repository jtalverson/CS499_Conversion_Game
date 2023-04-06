using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PickProblems : MonoBehaviour
{
    /*
    public class Problem // temporary class for developing purpose
    {
        public string Question1 { get; set; }
        public string Question2 { get; set; }
    }

    public List<Problem> problemsSet = new List<Problem>();
    public List<Problem> conversionProblemBank; // import from sheet
    public List<Problem> calculationProblemBank; // import from sheet
    public int conversionCount; // import based on level
    public int calculationCount; // import based on level
    */
    // Start is called before the first frame update
    void Start()
    {
        
    }
    /*
    public void pickProblemSet () 
    {
        // append randomly picked conversion problems to the set
        pickSomeProblems(conversionProblemBank, conversionCount);
        // append randomly picked calculation problems to the set
        pickSomeProblems(calculationProblemBank, calculationCount);
    }

    void pickSomeProblems (List<Problem> bank, int problemCount)
    {
        // shuffle first problemCount elements using all elements in list
        for (int i = 0; i < problemCount; i++) 
        {   
            var temp = bank[i]; // hold current problem
            // choose random from the current problem to the end of the bank
            int randomIndex = Random.Range(i, bank.Count);
            // swap the random chosen problem with the current problem
            bank[i] = bank[randomIndex]; 
            bank[randomIndex] = temp;
        }
        problemsSet.Add(bank.GetRange(0, problemCount));
    }
    */
    
}
