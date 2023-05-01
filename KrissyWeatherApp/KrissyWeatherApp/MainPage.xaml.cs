using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KrissyWeatherApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            GetCoodinates();
        }
        private string Location { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }

        private async void GetCoodinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude;
                    Longtitude = location.Longitude;
                    Location = await GetCity(location);
                    GetWeatherInfo();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task<string> GetCity(Location location)
        {
            var places = await Geocoding.GetPlacemarksAsync(location);
            var currentPlace = places?.FirstOrDefault();

            if (currentPlace != null)

                return $"{currentPlace.Locality},{currentPlace.CountryName}";

            return null;
        }

        private async void GetWeatherInfo()
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={Location}&appid=2c24436d9d9a44bc6d9eae99d7835bb9&units=metric";

            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {

                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.weather[0].icon}";
                    cityTxt.Text = weatherInfo.name.ToUpper();
                    temp.Text = weatherInfo.main.temp.ToString("0");
                    tempmin.Text = weatherInfo.main.temp_min.ToString("0");
                    tempmax.Text = weatherInfo.main.temp_max.ToString("0");
                    feelslike.Text = weatherInfo.main.feels_like.ToString("0") + "°";
                    humidity.Text = $"{weatherInfo.main.humidity}%";
                    windspeed.Text = $" {weatherInfo.wind.speed} m/s";
                    visability.Text = $"{weatherInfo.visibility}%";


                    DateTime sunriseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    .AddSeconds(weatherInfo.sys.sunrise)
    .ToLocalTime();
                    string sunriseTimeString = sunriseTime.ToString("h:mm tt");
                    sunrise.Text = sunriseTimeString;

                    DateTime sunsetTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    .AddSeconds(weatherInfo.sys.sunset)
    .ToLocalTime();
                    string sunsetTimeString = sunsetTime.ToString("h:mm tt");
                    sunset.Text = sunsetTimeString;


                    var dt = new DateTime().ToUniversalTime().AddSeconds(weatherInfo.dt);
                    dateTxt.Text = dt.ToString("dddd, MMM dd").ToUpper();

                    GetForecast();
                    GetForecast3Hours();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No weather information found", "OK");
            }
        }
        private async void GetForecast()
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=2c24436d9d9a44bc6d9eae99d7835bb9&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        //var date = DateTime.ParseExact(list.dt_txt, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");

                    dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");

                    dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");

                    dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourImg.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");

                    dayFiveTxt.Text = DateTime.Parse(allList[4].dt_txt).ToString("dddd");
                    dateFiveTxt.Text = DateTime.Parse(allList[4].dt_txt).ToString("dd MMM");
                    iconFiveImg.Source = $"w{allList[4].weather[0].icon}";
                    tempFiveTxt.Text = allList[4].main.temp.ToString("0");



                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }
        private async void GetForecast3Hours()
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=2c24436d9d9a44bc6d9eae99d7835bb9&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        //var date = DateTime.ParseExact(list.dt_txt, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    //DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(allList[0].dt_txt);
                    //string formattedTime = dateTimeOffset.AddHours(3).ToString("h:mm tt");
                    First3hour.Text = DateTime.Parse(allList[0].dt_txt).ToString("h:mm tt");
                    First3hourForecast.Text = allList[0].main.temp.ToString("0");

                    DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(allList[1].dt_txt);
                    string formattedTime = dateTimeOffset.AddHours(3).ToString("h:mm tt");
                    Second3hour.Text = formattedTime;
                    Second3hourForecast.Text = allList[1].main.temp.ToString("0");

                    DateTimeOffset dateTimeOffset2 = DateTimeOffset.Parse(allList[2].dt_txt);
                    string formattedTime2 = dateTimeOffset.AddHours(6).ToString("h:mm tt");
                    T3hour.Text = formattedTime2;
                    T3hourForecast.Text = allList[2].main.temp.ToString("0");

                    DateTimeOffset dateTimeOffset3 = DateTimeOffset.Parse(allList[3].dt_txt);
                    string formattedTime3 = dateTimeOffset.AddHours(9).ToString("h:mm tt");
                    F3hour.Text = formattedTime3;
                    F3hourForecast.Text = allList[3].main.temp.ToString("0");

                    DateTimeOffset dateTimeOffset4 = DateTimeOffset.Parse(allList[4].dt_txt);
                    string formattedTime4 = dateTimeOffset.AddHours(12).ToString("h:mm tt");
                    Fif3hour.Text = formattedTime4;
                    Fif3hourForecast.Text = allList[4].main.temp.ToString("0");

                    DateTimeOffset dateTimeOffset5 = DateTimeOffset.Parse(allList[5].dt_txt);
                    string formattedTime5 = dateTimeOffset.AddHours(15).ToString("h:mm tt");
                    S3hour.Text = formattedTime5;
                    S3hourForecast.Text = allList[5].main.temp.ToString("0");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }

        }
    }
}
