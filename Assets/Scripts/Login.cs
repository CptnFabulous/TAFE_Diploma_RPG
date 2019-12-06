using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

#region EMAIL STUFF
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
#endregion

public class Login : MonoBehaviour
{
    [Header("Screens")]
    public GameObject loginScreen;
    public GameObject createAccountScreen;
    //public GameObject changePasswordScreen;
    public GameObject forgotPasswordScreen;

    [Header("Login")]
    public InputField loginUsername;
    public InputField loginPassword;
    public Text loginErrorLog;

    [Header("Create account")]
    public int usernameCharacterLimit;
    public InputField createUsername;
    public InputField createEmail;
    public InputField createPassword;
    public InputField confirmCreatePassword;
    public Text createAccountErrorLog;
    /*
    [Header("Change password")]
    public InputField changeUsername;
    public InputField oldPassword;
    public InputField changePassword;
    public InputField confirmChangePassword;
    public Text changePasswordErrorLog;
    */
    [Header("Forgot password")]
    public InputField resetEmail;
    public InputField codeInput;
    public InputField resetPassword;
    public InputField confirmResetPassword;
    public Text resetPasswordErrorLog;
    public int portNumber = 8080; //80 25
    public int codeLength = 6;
    public bool caseSensitiveCode = true;
    string usernameForResetting;
    string validationCode;
    string passwordError = "";
    
    public void DisplayScreen(GameObject screen) // Disables all screens and displays specified screen
    {
        loginScreen.SetActive(false);
        createAccountScreen.SetActive(false);
        //changePasswordScreen.SetActive(false);
        forgotPasswordScreen.SetActive(false);

        screen.SetActive(true);
    }

    bool ValidatePassword(string password, string confirmPassword)
    {
        passwordError = ""; // Empty string for error log

        // Set of bools to determine which password criterias are met
        bool passwordMatches = false;
        bool passwordHasNumbers = false;
        bool passwordHasLowerCaseLetters = false;
        bool passwordHasUpperCaseLetters = false;

        // Checks if both passwords entered are the same
        if (password == confirmPassword)
        {
            passwordMatches = true;
            print("Passwords match.");
        }

        // Scans through password to check for numbers
        string numbers = "0123456789";
        for (int i = 0; i < numbers.Length; i++)
        {
            if (password.Contains(numbers[i]))
            {
                passwordHasNumbers = true;
            }
        }
        if (passwordHasNumbers == true)
        {
            print("Password contains numbers.");
        }

        // Scans through password to check for lower-case letters
        string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        for (int i = 0; i < lowerCase.Length; i++)
        {
            if (password.Contains(lowerCase[i]))
            {
                passwordHasLowerCaseLetters = true;
            }
        }
        if (passwordHasLowerCaseLetters == true)
        {
            print("Password contains lower case letters.");
        }

        // Scans through password to check for upper-case letters
        string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < upperCase.Length; i++)
        {
            if (password.Contains(upperCase[i]))
            {
                passwordHasUpperCaseLetters = true;
            }
        }
        if (passwordHasUpperCaseLetters == true)
        {
            print("Password contains upper case letters.");
        }

        // If all bools are true, password is validated
        if (passwordMatches == true && passwordHasNumbers == true && passwordHasLowerCaseLetters == true && passwordHasUpperCaseLetters == true)
        {
            return true;
        }

        // Code past this point only runs if the bool does not return true. Generates error message
        if (passwordMatches == false)
        {
            passwordError += "Passwords do not match!";
            passwordError += "\n";
        }
        if (passwordHasNumbers == false)
        {
            passwordError += "Password does not contain numbers!";
            passwordError += "\n";
        }
        if (passwordHasLowerCaseLetters == false)
        {
            passwordError += "Password does not contain lower case letters!";
            passwordError += "\n";
        }
        if (passwordHasUpperCaseLetters == false)
        {
            passwordError += "Password does not contain upper case letters!";
            passwordError += "\n";
        }
        return false;
    }

    #region Create account
    public void CreateNewUser()
    {
        // Checks if password is valid and if username does not exceed character limit
        if (ValidatePassword(createPassword.text, confirmCreatePassword.text) && createUsername.text.Length <= usernameCharacterLimit)
        {
            // Run function to add new user to the database
            StartCoroutine(CreateUser(createUsername.text, createEmail.text, createPassword.text));
        }
        else
        {
            // Display error log
            string errorMessage = "";
            if (ValidatePassword(createPassword.text, confirmCreatePassword.text) == false)
            {
                errorMessage += passwordError;
            }
            if (createUsername.text.Length > usernameCharacterLimit)
            {
                errorMessage += "Username character limit exceeded!";
                errorMessage += "\n";
            }
            createAccountErrorLog.text = errorMessage;
        }
    }

    IEnumerator CreateUser(string username, string email, string password)
    {
        string createUserURL = "http://localhost/nsirpg/insertuser.php"; // Accesses appropriate PHP file to add a user to the database
        WWWForm form = new WWWForm(); // Creates new web form
        form.AddField("username", username); // Adds username to form
        form.AddField("email", email); // Adds email to form
        form.AddField("password", password); // Adds password to form
        UnityWebRequest webRequest = UnityWebRequest.Post(createUserURL, form); // Submits form
        yield return webRequest.SendWebRequest(); // Waits while form is processed
        Debug.Log(webRequest.downloadHandler.text);
        createAccountErrorLog.text = webRequest.downloadHandler.text; // Displays log output
    }
    #endregion

    #region Login
    public void SubmitLogin()
    {
        StartCoroutine(UserLogin(loginUsername.text, loginPassword.text));
    }

    IEnumerator UserLogin(string username, string password)
    {
        string loginURL = "http://localhost/nsirpg/login.php"; // Accesses appropriate PHP file to log in with an account
        WWWForm form = new WWWForm(); // Accesses appropriate PHP file to add a user to the database
        form.AddField("username", username); // Adds username to form
        form.AddField("password", password); // Adds password to form
        UnityWebRequest webRequest = UnityWebRequest.Post(loginURL, form); // Submits form
        yield return webRequest.SendWebRequest(); // Waits while form is processed
        Debug.Log(webRequest.downloadHandler.text);
        if (webRequest.downloadHandler.text == "Login successful") // Checks log output, if correct message, starts function to load game
        {
            //LoadGame(username);
            StartCoroutine(LoadGame(username));
        }
        else
        {
            loginErrorLog.text = webRequest.downloadHandler.text; // If login fails, display error log
        }
    }
    #endregion

    #region Reset password
    public IEnumerator UpdatePassword(string username, string createPassword)
    {
        string updatePasswordURL = "http://localhost/nsirpg/updatepassword.php"; // Accesses appropriate PHP file to update the user's password
        WWWForm form = new WWWForm(); // Accesses appropriate PHP file to add a user to the database
        form.AddField("username_Post", username); // Adds username to form
        form.AddField("password_Post", createPassword); // Adds password to form
        UnityWebRequest webRequest = UnityWebRequest.Post(updatePasswordURL, form); // Submits form
        yield return webRequest.SendWebRequest(); // Waits while form is processed
        Debug.Log(webRequest.downloadHandler.text);
    }
    #endregion

    #region Forgot password
    public void InputResetEmail() // This function simply allows a coroutine to be run as an event from clicking a button
    {
        StartCoroutine(CheckEmail(resetEmail.text));
    }

    public IEnumerator CheckEmail(string email)
    {
        string checkEmailURL = "http://localhost/nsirpg/checkemail.php"; // Accesses appropriate PHP file to check email
        WWWForm form = new WWWForm(); // Accesses appropriate PHP file to add a user to the database
        form.AddField("email_Post", email); // Adds email to form
        UnityWebRequest webRequest = UnityWebRequest.Post(checkEmailURL, form); // Submits form
        yield return webRequest.SendWebRequest(); // Waits while form is processed
        Debug.Log(webRequest.downloadHandler.text);

        if (webRequest.downloadHandler.text != "User not found") // If form is processed successfully
        {
            // Obtain username from web request form
            usernameForResetting = webRequest.downloadHandler.text;
            print(usernameForResetting);
            // Send email to appropriate user's email address
            SendResetEmail(resetEmail.text, usernameForResetting);
        }
        else
        {
            //createAccountToolTip.text = webRequest.downloadHandler.text; Add tooltip to say user not found
            print(webRequest.downloadHandler.text);
        }
    }

    public void SendResetEmail(string _email, string _username)
    {
        validationCode = GenerateCode(codeLength, caseSensitiveCode); // Generate a randomised code for the user to reset their password with
        print(validationCode);
        MailMessage mail = new MailMessage(); // Creates a new email to send to the user
        mail.From = new MailAddress("sqlunityclasssydney@gmail.com");
        mail.To.Add(_email);
        mail.Subject = "NSIRPG Password Reset";
        mail.Body = "Hello, " + _username + ",\nReset your password using this code: " + validationCode; // Lists player's username and randomly-generated code
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = portNumber;
        smtpServer.Credentials = new NetworkCredential("sqlunityclasssydney@gmail.com", "sqlpassword") as ICredentialsByHost; // MAKE PASSWORD AND EMAIL INTO PUBLIC VARIABLES
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors) { return true; };
        smtpServer.Send(mail); // Sends email
        Debug.Log("Sending email");
    }
    
    /*
    string GenerateCode(int length, bool caseSensitive)
    {
        string[] digits = new string[length];
        string finalCode = "";
        for (int i = 0; i < length; i++)
        {
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (caseSensitive == true)
            {
                characters += "abcdefghijklmnopqrstuvwxyz";
            }
            //digits[i] = characters[UnityEngine.Random.Range(0, characters.Length)].ToString();
            digits[i] = characters[Mathf.RoundToInt(UnityEngine.Random.Range(0, characters.Length))].ToString();
            finalCode += digits[i];
        }
        return finalCode;
    }
    */
    string GenerateCode(int length, bool caseSensitive)
    {
        // Generates a list of characters, and adds an extra section of lower case letters if caseSensitive is set to true
        string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (caseSensitive == true)
        {
            characters += "abcdefghijklmnopqrstuvwxyz";
        }

        string finalCode = ""; // Declares an empty string to add appropriate variables to
        for (int i = 0; i < length; i++) // Adds a new character the appropriate number of times based on the length integer
        {
            // Randomly selects a character from the characters string, converts it to a string and adds it to the end of the finalCode string.
            string character = characters[UnityEngine.Random.Range(0, characters.Length - 1)].ToString();
            finalCode += character;
        }
        return finalCode;
    }

    public void CheckCode()
    {
        // Checks to validate password, and that the code entered is identical to the one generated
        if (ValidatePassword(resetPassword.text, confirmResetPassword.text) == true && codeInput.text == validationCode)
        {
            // Updates player's password
            StartCoroutine(UpdatePassword(usernameForResetting, resetPassword.text));
        }
        else
        {
            // Generates new error message, with information on if the code matched or not
            string passwordResetError = passwordError;
            if (codeInput.text != validationCode)
            {
                passwordResetError += "Verification code does not match the one sent to the email.";
            }
            resetPasswordErrorLog.text = passwordResetError;
        }
    }
    #endregion

    IEnumerator LoadGame(string username)
    {
        print("Loading scene");
        LoadGameData lgd = GetComponent<LoadGameData>(); // Finds LoadGameData script attached to same GameObject as this script
        SceneManager.LoadScene(lgd.gameSceneName); // Loads game level as specified in LoadGameData
        yield return new WaitForEndOfFrame(); // Waits until frame has ended and new scene has loaded
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint"); // Finds SpawnPoint object in game level
        StartCoroutine(SaveAndLoad.LoadCharacter(username, lgd.playerPrefab, spawnPoint.transform)); // Loads player character into scene, using username to load appropriate inventory data
    }
}