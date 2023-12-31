﻿using System;
using System.Windows;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Data;
using System.Windows.Documents;
using BMICalc.Classes;
using System.Collections.Generic;
using static BMICalc.MainWindow;

namespace BMICalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    [XmlRoot("BMICalc", Namespace = "www.bmicalc.amog")]
    public partial class MainWindow : Window
    {
        public string FilePath = "../../../BMICalc/";
        public string FileName = "yourBMI.xml";

        XmlSerializer xmlSerializer;
        List<UserBMI> userBmiList;

        public class Customer
        {
            [XmlAttribute("Last Name")]
            public string lastName { get; set; }
            [XmlAttribute("First Name")]
            public string firstName { get; set; }
            [XmlAttribute("Phone Number")]
            public string phoneNumber { get; set; }
            [XmlAttribute("Height")]
            public int heightInches { get; set; }
            [XmlAttribute("Inches")]
            public int weightLbs { get; set; }
            [XmlAttribute("Customer BMI")]
            public int custBMI { get; set; }
            [XmlAttribute("Status")]
            public string statusTitle { get; set; }

        }
        public MainWindow()
        {
            InitializeComponent();
            xmlSerializer = new XmlSerializer(typeof(List<UserBMI>));
            userBmiList = new List<UserBMI>();
            OnLoadStats();
        }

        #region Part 1 of the Lab. ClearBtn & ExitBtn
        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            xPhoneBox.Text = "";
            xFirstNameBox.Text = "";
            xLastNameBox.Text = "";
            xHeightInchesBox.Text = "";
            xWeightLbsBox.Text = "";
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = new Customer();

            customer.lastName = xLastNameBox.Text;
            customer.firstName = xFirstNameBox.Text;
            customer.phoneNumber = xPhoneBox.Text;

            int currentWeight = 0;
            int currentHeight = 0;

            Int32.TryParse(xWeightLbsBox.Text, out currentWeight);
            Int32.TryParse(xHeightInchesBox.Text, out currentHeight);

            customer.weightLbs = currentWeight;
            customer.heightInches = currentHeight;

            int bmi;
            bmi = 703 * customer.weightLbs / (customer.heightInches * customer.heightInches);
            customer.custBMI = bmi;

            // MessageBox.Show($"The Customers name is {customer.firstName} {customer.lastName} and they can be reached at {customer.phoneNumber}. They are {customer.heightInches} tall. Their weight is {customer.weightLbs} lbs. Their BMI is {customer.custBMI}.");

            xYourBMIResults.Text = customer.custBMI.ToString();

            if (customer.custBMI < 18.5)
            {
                xBMIMessage.Text = "According to CDC.gov BMI Calculator you are underweight.";
                customer.statusTitle = "UnderWeight";
            }
            else if (customer.custBMI < 24.9)
            {
                xBMIMessage.Text = "According to CDC.gov BMI Calculator you have a normal body weight.";
                customer.statusTitle = "Normal";
            }
            else if (customer.custBMI < 29.9)
            {
                xBMIMessage.Text = "According to CDC.gov BMI Calculator you are overweight.";
                customer.statusTitle = "Overweight";
            }
            else
            {
                xBMIMessage.Text = "According to CDC.gov BMI Calculator you are obese.";
                customer.statusTitle = "Obese";
            }

            try
            {
                FileStream fileStream = new FileStream(FilePath + FileName, FileMode.Create, FileAccess.Write);
                UserBMI userBMI = new UserBMI();
                userBMI.FirstName = customer.firstName;
                userBMI.LastName = customer.lastName;
                userBMI.PhoneNumber = customer.phoneNumber;
                userBMI.HeightInInches = customer.heightInches;
                userBMI.BmiResults = customer.custBMI;
                userBMI.Status = customer.statusTitle;

                userBmiList.Add(userBMI);

                xmlSerializer.Serialize(fileStream, userBmiList);
                fileStream.Close();

            }
            catch (FileNotFoundException)
            {
                FileStream fileStream = new FileStream(FilePath + FileName, FileMode.Create);
                fileStream.Close();
            }


            OnLoadStats();
        }

        private void OnLoadStats()
        {
            try
            {
                FileStream fileStream = new FileStream(FilePath + FileName, FileMode.Open, FileAccess.Read);
                userBmiList = (List<UserBMI>)xmlSerializer.Deserialize(fileStream);

                xDataGrid.ItemsSource = userBmiList;
                fileStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
    }
}
