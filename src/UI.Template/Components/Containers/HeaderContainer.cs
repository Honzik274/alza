using OpenQA.Selenium;
using UI.Template.Components.Basic;
using UI.Template.Framework.Extensions;
using UI.Template.Pages;

namespace UI.Template.Components.Containers;

public class HeaderContainer : BaseComponent
{
    private readonly Simple OpenBasketButton = new(By.XPath("//*[@class='cart-widget']"));
    private readonly Simple BasketCount = new(By.XPath("//*[@class='cart-count']"));
    private readonly Simple BasketContainer = new(By.XPath("//*[@ko-id='cart-body']"));
    private readonly Button ClearBasketButton = new(By.XPath("//button[@class='clear-cart']"));
    private readonly Button CloseBasketButton = new(By.XPath("//button[@class='close-cart']"));
    private readonly Simple Title = new(By.XPath("//h1[@class='shop-title']"));

    // NOVÉ tlačítko Checkout
    private readonly Button CheckoutButton = new(By.XPath("//button[@class='checkout-button']"));

    /// <summary>
    /// Opens the basket container.
    /// </summary>
    public void OpenBasketContainer()
    {
        if (BasketContainer.IsNotDisplayed())
        {
            OpenBasketButton.Click();
            BasketContainer.WaitForDisplayed();
        }
    }

    public int GetBasketCount()
    {
        if (BasketCount.IsNotDisplayed())
        {
            return 0;
        }

        return int.TryParse(BasketCount.GetText(), out int count) ?
                            count :
                            throw new InvalidOperationException("Failed to parse basket count.");
    }

    public void CloseBasketContainer()
    {
        if (BasketContainer.IsDisplayed())
        {
            CloseBasketButton.Click();
            BasketContainer.WaitForNotDisplayed();
        }
    }

    public List<string> GetProductNamesInBasket()
    {
        if (BasketContainer.IsNotDisplayed())
        {
            throw new InvalidOperationException("Basket container is not open. Please open the basket before retrieving product names.");
        }

        List<string> productNames = new List<string>();
        By productPathLocator = By.XPath("//*[@class='cart-item-details']");

        int productCardsCount = WebDriver.FindElements(productPathLocator).Count;
        for (int i = 1; i <= productCardsCount; i++)
        {
            Simple productCard = new(By.XPath($"({productPathLocator.ToSelector()}/h3)[{i}]"));
            productCard.ScrollTo();
            productNames.Add(productCard.GetText());
        }

        return productNames;
    }

    public bool GetNthProduct(int n, out string productName, out string productDetail)
    {
        productName = string.Empty;
        productDetail = string.Empty;

        string nthProductXPathSelector = $"(//*[@class='cart-item-details'])[{n}]";

        Simple nthProduct = new(By.XPath(nthProductXPathSelector));
        Simple nthProductName = new(By.XPath($"{nthProductXPathSelector}/h3"));
        Simple nthProductDetail = new(By.XPath($"{nthProductXPathSelector}/p"));

        if (BasketContainer.IsDisplayed() && nthProduct.IsDisplayed())
        {
            productName = nthProductName.GetText();
            productDetail = nthProductDetail.GetText();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Clicks the Checkout button in the basket modal and returns CheckoutPage.
    /// </summary>
    public CheckoutPage GoToCheckout()
    {
        // otevře modal, pokud není otevřený
        if (BasketContainer.IsNotDisplayed())
        {
            OpenBasketButton.Click();
            BasketContainer.WaitForDisplayed();
        }

        // klikne na Checkout button
        CheckoutButton.Click();

        // vrátí instanci CheckoutPage
        return new CheckoutPage();
    }

    public AdminPage OpenAdminPage()
    {
        WebDriver.WaitForUrlChanged(() => Title.Click());
        return new AdminPage();
    }
}
