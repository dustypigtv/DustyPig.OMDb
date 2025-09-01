using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DustyPig.OMDb.Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        //Superman (2025)
        var response = await ClientFactory.SharedClient.GetMovieByIdAsync("tt5950044");
        Assert.AreEqual("Superman", response.Data.Title, true);
        Assert.AreEqual("movie", response.Data.Type);
    }


    [TestMethod]
    public async Task TestMethod2()
    {
        //The Avengers (2012)
        var response = await ClientFactory.SharedClient.GetMovieByIdAsync("tt0848228");
        Assert.AreEqual("The Avengers", response.Data.Title, true);
        Assert.AreEqual("movie", response.Data.Type);
    }

    [TestMethod]
    public async Task TestMethod3()
    {
        //Buffy the Vampire Slayer
        var response = await ClientFactory.SharedClient.GetMovieByIdAsync("tt0118276");
        Assert.AreEqual("Buffy the Vampire Slayer", response.Data.Title, true);
        Assert.AreEqual("series", response.Data.Type);
    }

    [TestMethod]
    public async Task TestMethod4()
    {
        //Doctor Who (2005)
        var response = await ClientFactory.SharedClient.GetSeriesByIdAsync("tt0436992");
        Assert.AreEqual("Doctor Who", response.Data.Title, true);
        Assert.AreEqual("series", response.Data.Type);
    }

    [TestMethod]
    public async Task TestMethod5()
    {
        //Doctor Who (2005) - s03e10 - Blink
        var response = await ClientFactory.SharedClient.GetEpisodeByIdAsync("tt1000252");
        Assert.AreEqual("tt0436992", response.Data.SeriesId);
        Assert.AreEqual("Blink", response.Data.Title, true);
        Assert.AreEqual("episode", response.Data.Type);
    }


    [TestMethod]
    public async Task TestMethod6()
    {
        //Search: The Avengers (2012)
        var response = await ClientFactory.SharedClient.SearchForMovieAsync("the avengers", 2012);
        var item = response.Data.Search.First(_ => _.Title.Equals("The Avengers", StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual("movie", item.Type);
        Assert.AreEqual("2012", item.Year);
    }

    [TestMethod]
    public async Task TestMethod7()
    {
        //Search: Doctor Who (2005)
        var response = await ClientFactory.SharedClient.SearchForSeriesAsync("doctor who", 2005);
        var item = response.Data.Search
            .Where(_ => _.Type == "series")
            .Where(_ => _.Title.Equals("Doctor Who", StringComparison.OrdinalIgnoreCase))
            .First();

        Assert.IsTrue(item.Year.Contains("2005"));
    }


}
