using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using System;
using System.Timers;
using SQLite;
using System.IO;
using System.Collections.Generic;

namespace Punch_In_App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Timer aTimer;
        TextView texti;
        TextView texti2;
        string dataContents;

        SQLiteConnection db;
        DbTableTemplate currentTable;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            //find time text UI
            texti = FindViewById<TextView>(Resource.Id.timeText);
            texti2 = FindViewById<TextView>(Resource.Id.databaseText);
            //set the time to the text on startup
            string localDate = DateTime.Now.ToString("hh:mm tt");
            texti.Text = localDate;
            // Create a timer with a small interval.
            aTimer = new Timer(100);
            // Hook up the Elapsed event for the timer. And Auto Start it 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            //hook into the button and fire a click event
            Button button = FindViewById<Button>(Resource.Id.button1);
            button.Click += Button_Click;

            //drop down menu code
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.planets_array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            //FIX NAMES
            Spinner spinner1 = FindViewById<Spinner>(Resource.Id.spinner1);

            spinner1.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected1);
            var adapter1 = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.planets_array1, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner1.Adapter = adapter1;

            //view database on startup
            CreateAndAccessTable();

            List<DbTableTemplate> table = db.Table<DbTableTemplate>().ToList();
            table.ForEach(x => dataContents += x.currentDate + " - " + x.clockIn + " " + x.breakOut + " " + x.breakIn + " " + x.clockOut + " | " + x.TotalWorkedHours + "\n");
            texti2.Text = dataContents;
            dataContents = string.Empty;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {

        }

        private void spinner_ItemSelected1(object sender, AdapterView.ItemSelectedEventArgs e)
        {

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            string localDate = DateTime.Now.ToString("hh:mm tt");

            RunOnUiThread(() =>
            {
                texti.Text = localDate;
            });
        }

        private void Button_Click(object sender, EventArgs e)
        {
            CreateAndAccessTable();

            if (ClockedIn())
            {
                if (OutForBreak())
                {
                    if (InForBreak())
                    {
                        if (ClockedOut())
                        {
                            CalculateHours();
                        }
                    }
                }
            }
            List<DbTableTemplate> table = db.Table<DbTableTemplate>().ToList();

            table.ForEach(x => dataContents += x.currentDate + " - " + x.clockIn + " " + x.breakOut + " " + x.breakIn + " " + x.clockOut + " | " + x.TotalWorkedHours + "\n");

            texti2.Text = dataContents;
            dataContents = string.Empty;

            //db.DropTable<DbTableTemplate>();
        }

        void CreateAndAccessTable()
        {
            //path string for database file
            string dbpath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "dbClock.db3");
            //setup connection to database file
            db = new SQLiteConnection(dbpath);
            //create a table
            db.CreateTable<DbTableTemplate>();

            bool dateAlreadyMade = db.Table<DbTableTemplate>().ToList().Exists(x => x.currentDate.Equals(DateTime.Now.ToString("MM/dd/yy")));
            if (dateAlreadyMade)
            {
                currentTable = db.Table<DbTableTemplate>().ToList().Find(x => x.currentDate.Equals(DateTime.Now.ToString("MM/dd/yy")));
            }
            else
            {
                DbTableTemplate newTable = new DbTableTemplate();
                newTable.currentDate = DateTime.Now.ToString("MM/dd/yy");
                db.Insert(newTable);
            }
        }

        bool ClockedIn()
        {
            currentTable = db.Table<DbTableTemplate>().ToList().Find(x => x.currentDate.Equals(DateTime.Now.ToString("MM/dd/yy")));

            if (currentTable.clockIn != string.Empty)
            {
                return true;
            }
            else
            {
                currentTable.clockIn = DateTime.Now.ToString("hh:mm tt");
                db.Update(currentTable);
                return false;
            }
        }

        bool OutForBreak()
        {
            currentTable = db.Table<DbTableTemplate>().ToList().Find(x => x.currentDate.Equals(DateTime.Now.ToString("MM/dd/yy")));

            if (currentTable.breakOut != string.Empty)
            {
                return true;
            }
            else
            {
                currentTable.breakOut = DateTime.Now.ToString("hh:mm tt");
                db.Update(currentTable);
                return false;
            }
        }

        bool InForBreak()
        {
            currentTable = db.Table<DbTableTemplate>().ToList().Find(x => x.currentDate.Equals(DateTime.Now.ToString("MM/dd/yy")));

            if (currentTable.breakIn != string.Empty)
            {
                return true;
            }
            else
            {
                currentTable.breakIn = DateTime.Now.ToString("hh:mm tt");
                db.Update(currentTable);
                return false;
            }
        }

        bool ClockedOut()
        {
            currentTable = db.Table<DbTableTemplate>().ToList().Find(x => x.currentDate.Equals(DateTime.Now.ToString("MM/dd/yy")));

            if (currentTable.clockOut != string.Empty)
            {
                return true;
            }
            else
            {
                currentTable.clockOut = DateTime.Now.ToString("hh:mm tt");
                db.Update(currentTable);
                return false;
            }
        }

        void CalculateHours()
        {
            currentTable = db.Table<DbTableTemplate>().ToList().Find(x => x.currentDate.Equals(DateTime.Now.ToString("MM/dd/yy")));
            DateTime parsedClockIn = DateTime.Parse(currentTable.clockIn);
            DateTime parsedBreakOut = DateTime.Parse(currentTable.breakOut);
            DateTime parsedBreakIn = DateTime.Parse(currentTable.breakIn);
            DateTime parsedClockOut = DateTime.Parse(currentTable.clockOut);

            TimeSpan finalTime = parsedBreakOut.Subtract(parsedClockIn).Add(parsedClockOut.Subtract(parsedBreakIn));
            currentTable.TotalWorkedHours = finalTime.ToString(@"hh\:mm");
            db.Update(currentTable);
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("Timer Stopped");
            aTimer.Elapsed -= OnTimedEvent;
            base.OnStop();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
