using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MilkAnalyzerTest.DataAccess;

namespace MilkAnalyzerTest.Tests
{
    public static class SerialSimulationTest
    {
        // Run an async simulation that inserts a master record and several value rows for given profileId.
        public static async Task<long> RunSimulationAsync(int profileId)
        {
            // Ensure connection string exists (fall back to local SQLEXPRESS same as Program.cs)
            if (string.IsNullOrEmpty(Database.ConnectionString))
            {
                Database.ConnectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=MilkAnalyzerTest;Integrated Security=True;TrustServerCertificate=True;";
            }

            // Simulated raw message from serial device
            var rawMessage = "SIMTEST|ID=1001;FAT=3.5;PROTEIN=3.2;LACTOSE=4.6";

            // Create sample parsed values using KeyName values that match seeded MilkAnalyzerParameters
            var values = new List<(string name, string value, string type)>
            {
                ("dummy_fat", "3.5", "float"),
                ("dummy_protein", "3.2", "float"),
                ("dummy_lactose", "4.6", "float"),
            };

            // Insert using existing Database helper (pass profileId)
            var id = await Database.InsertMilkTestResultAsync(rawMessage, values, profileId);
            return id;
        }

        // Convenience method to run and show result via MessageBox (call from UI)
        public static async void RunAndNotify(int profileId)
        {
            try
            {
                var id = await RunSimulationAsync(profileId);
                MessageBox.Show($"Test data inserted. Master id: {id}", "Test Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Test failed: {ex.Message}", "Test Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
