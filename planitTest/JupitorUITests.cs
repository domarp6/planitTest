using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace planitTest;

public class Tests
{
    IWebDriver driver;

    [SetUp]
    public void Start_Browser()
    {
        driver = new ChromeDriver();
        driver.Manage().Window.Maximize();

        //loading the page and wait
        driver.Navigate().GoToUrl("https://jupiter.cloud.planittesting.com/#/");
        
    }

    [Test]
    public void ContactPage_Test1()
    {

        WebDriverWait wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(30));
        wait.Until(c => c.FindElement(By.XPath("//a[normalize-space()='Start Shopping »']")));

        //click the Contact.
        IWebElement btnContact = driver.FindElement(By.XPath("//a[normalize-space()='Contact']"));
        btnContact.Click();

        //Click submit button to see error messages
        wait.Until(c => c.FindElement(By.XPath("//a[normalize-space()='Submit']")).Enabled);
        IWebElement btnSubmit = driver.FindElement(By.XPath("//a[normalize-space()='Submit']"));
        btnSubmit.Click();
        Thread.Sleep(2000);

        //Validate Error Messages
        Assert.IsTrue(driver.FindElement(By.Id("forename-err")).Displayed, "ForeName Error Message is not showing" );
        Assert.IsTrue(driver.FindElement(By.Id("email-err")).Displayed, "Email Error Message is not showing");
        Assert.IsTrue(driver.FindElement(By.Id("message-err")).Displayed, "Message Error Message is not showing");

        //Fill mandatory fields
        driver.FindElement(By.Id("forename")).SendKeys("John");
        driver.FindElement(By.Id("email")).SendKeys("John@something.com");
        driver.FindElement(By.Id("message")).SendKeys("John is here to test the webpage");

        //Verifying the error messages are not showing anymore
        Assert.IsFalse(driver.FindElements(By.XPath("//span[contains(@id,'-err')]")).Count > 0, "Still showing the error messages");
        
       
        Assert.Pass();
    }

    [Test]
    public void ContactPage_Test2()
    {
       
        WebDriverWait wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(30));
        wait.Until(c => c.FindElement(By.XPath("//a[normalize-space()='Start Shopping »']")));

        //click the Contact.
        IWebElement btnContact = driver.FindElement(By.XPath("//a[normalize-space()='Contact']"));
        btnContact.Click();

        //wait for the page load
        wait.Until(c => c.FindElement(By.XPath("//a[normalize-space()='Submit']")).Enabled);
        

        //Fill mandatory fields
        driver.FindElement(By.Id("forename")).SendKeys("John");
        driver.FindElement(By.Id("email")).SendKeys("John@something.com");
        driver.FindElement(By.Id("message")).SendKeys("John is here to test the webpage");

        //Verifying the error messages are not showing anymore
        Assert.IsFalse(driver.FindElements(By.XPath("//span[contains(@id,'-err')]")).Count > 0, "Still showing the error messages");

        IWebElement btnSubmit = driver.FindElement(By.XPath("//a[normalize-space()='Submit']"));
        btnSubmit.Click();

        //wait unit the Back button is displayed
        Assert.IsTrue(wait.Until(c => c.FindElement(By.XPath("//a[normalize-space()='« Back']")).Displayed), "Back button is not shown after submit and waiting for 30sec");


        Assert.Pass();
    }

    [Test]
    public void ShoppingPage_Test3()
    {

        WebDriverWait wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(20));
        wait.Until(c => c.FindElement(By.XPath("//a[normalize-space()='Start Shopping »']")));

        //Click start shopping
        driver.FindElement(By.XPath("//a[normalize-space()='Start Shopping »']")).Click();
        wait.Until(c => c.FindElement(By.XPath("//li[@class='product ng-scope']")).Displayed);

        //select shopping items
        this.AddProductTo_Cart("Stuffed Frog", 2);
        this.AddProductTo_Cart("Fluffy Bunny", 5);
        this.AddProductTo_Cart("Valentine Bear", 3);

        //Click on cart page and wait for the page load
        driver.FindElement(By.XPath("//a[@href='#/cart']")).Click();
        wait.Until(c => c.FindElement(By.XPath("//a[normalize-space()='Check Out']")));
        Thread.Sleep(4000);

        //Validate subtotal, items count and overall total
        this.ValidateCart("Stuffed Frog", "2", "21.98");
        this.ValidateCart("Fluffy Bunny", "5", "49.95");
        this.ValidateCart("Valentine Bear", "3", "44.97");
        Assert.IsTrue(driver.FindElement(By.XPath("//strong[contains(@class,'total ng-binding')]")).Text.Contains("116.9"), "Cart SubTotal was not as expected in the cart");

        Assert.Pass();

    }

    /// <summary>
    /// Function to validate products in cart
    /// </summary>
    /// <param name="productName">Name of the Product</param>
    /// <param name="exQty">expected Quantity</param>
    /// <param name="exTotal">expected Total</param>
    public void ValidateCart(string productName, string exQty, string exTotal)
    {
        var productQty = driver.FindElement(By.XPath("//td[normalize-space()='"+productName+"']/following-sibling::td[2]/child::input")).GetAttribute("value");
        var productTotal = driver.FindElement(By.XPath("//td[normalize-space()='"+productName+"']/following-sibling::td[3]")).Text;

        Assert.IsTrue(exQty == productQty, "Product Quantity was not as expected in the Cart.");
        Assert.IsTrue(productTotal.Contains(exTotal), "Product SubTotal was not as expected in the Cart.");
    }

    /// <summary>
    /// Function to add items to cart. 
    /// </summary>
    /// <param name="itemName">Name of the product</param>
    /// <param name="nums">Number of items to add</param>
    public void AddProductTo_Cart(string itemName, int nums)
    {
        string item_XPath = "//h4[normalize-space()='"+ itemName +"']//following-sibling::p/a";

        IWebElement itemToAdd = driver.FindElement(By.XPath(item_XPath));

        if (itemToAdd.Displayed)
        {
            for (int i = 0; i < nums; i++)
            {
                itemToAdd.Click();
                Thread.Sleep(500);
            }
        }
    }

    [TearDown]
    public void Close_Browser()
    {
        driver.Quit();
    }
}
