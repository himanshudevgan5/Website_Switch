using System.Windows.Forms;

namespace BrowserSwitch
{
    public class Tab : Website
    {
        public WebBrowser webobject;
        public TabPage tabobject;
        public Label labelSeconds;
        public Label labelCount;
        public Label labelNoInternetConnection;
        public WebBrowser checkBrowser = null;



        public Tab(Website website)
        {

            this.ID = website.ID;
            if (website.PageTitle == null)
            {
                throw new System.ArgumentException("Argument PageTitle has to be set in xml for ID " + website.ID.ToString());
            }
            else if (website.URL == null)
            {
                throw new System.ArgumentException("Argument URL has to be set in xml for ID " + website.ID.ToString());
            }
            else if (website.URLType == null)
            {
                throw new System.ArgumentException("Argument URLType has to be set in xml for ID " + website.ID.ToString());
            }
            else if (website.ErrorURL == null)
            {
                throw new System.ArgumentException("Argument ErrorURL has to be set in xml for ID " + website.ID.ToString());
            }
            else if (website.ErrorURLType == null)
            {
                throw new System.ArgumentException("Argument ErrorURLType has to be set in xml for ID " + website.ID.ToString());
            }
            else
            {
                this.PageTitle = website.PageTitle;
                this.URL = website.URL;
                this.URLType = website.URLType;
                this.ZoomLevel = website.ZoomLevel;
                this.ErrorURL = website.ErrorURL;
                this.ErrorURLType = website.ErrorURLType;
                this.ZoomLevelErrorURL = website.ZoomLevelErrorURL;
            }

    }
}




}
