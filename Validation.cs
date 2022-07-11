namespace ValidationClass
{
    public static class Validation
    {
        /// <summary>
        /// VALIDATING INPUTED MENU NUMBER
        /// </summary>
        public static bool ValidatesMenuInputNumber(Int32 minNumber, Int32 maxNumber, string strMenuInput)
        {
            Int32 chosenNumber = 0;

            //CHECKING IF THE INPUT IS NULL
            if (strMenuInput is null) {
                return false;
            }

            //VALIDATING IF THE INPUT IS VALID NUMBER
            if (!int.TryParse(strMenuInput, out chosenNumber)) {
                return false;
            }

            //VALIDATING IF THE INPUT IS IN BETWEEN THE RANGE
            if (chosenNumber < minNumber || chosenNumber > maxNumber) {
                return false;
            }

            return true;
        }   
        
    }
}