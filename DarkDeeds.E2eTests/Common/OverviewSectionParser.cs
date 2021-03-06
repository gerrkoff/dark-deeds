﻿using System.Text;
using OpenQA.Selenium;

namespace DarkDeeds.E2eTests.Common
{
    public class OverviewSectionParser
    {
        private readonly IWebElement _section;
        private StringBuilder _query = new StringBuilder(".");
        
        public OverviewSectionParser(IWebElement section)
        {
            _section = section;
        }

        public OverviewSectionParser FindBlock(int order)
        {
            _query.Append($"//div{Xpath.ClassContains("days-block")}[{order}]");
            return this;
        }

        public OverviewSectionParser FindDay(int order)
        {
            _query.Append($"//div{Xpath.ClassContains("days-block-item")}[{order}]");
            return this;
        }
        
        public OverviewSectionParser FindHeader()
        {
            _query.Append($"//span{Xpath.ClassContains("day-card-header")}");
            return this;
        }
        
        public OverviewSectionParser FindTask(string text)
        {
            _query.Append($"//{Xpath.TaskWithText(text)}");
            return this;
        }

        public int CountExpiredDays()
        {
            string expiredDayXpath = $"//div{Xpath.ClassContains("day-card-expired")}";
            return _section.FindElements(By.XPath(expiredDayXpath)).Count;
        }

        public IWebElement GetElement()
        {
            var element = _section.FindElement(By.XPath(_query.ToString()));
            _query = new StringBuilder(".");
            return element;
        } 
    }
}