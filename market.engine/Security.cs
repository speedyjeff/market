using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine
{
    public struct Security
    {
        public readonly SecurityNames Name;
        public readonly string Fullname;
        public readonly int Yield;
        public readonly string Description;

        public static Security ByName(SecurityNames name)
        {
            if ((int)name < 0 || (int)name > All.Length) throw new Exception("invalid security name");
            return All[(int)name];
        }

        #region private
        private Security(SecurityNames name, string fullname, int yield, string description)
        {
            Name = name;
            Fullname = fullname;
            Yield = yield;
            Description = description;
        }

        private static Security[] All = new Security[]
            {
                // SecurityNames.CentralCityMunicipalBonds,
                new Security(name: SecurityNames.CentralCityMunicipalBonds,
                    fullname: "Central City Municipal Bonds",
                    yield: 5,
                    description: "An AAA bond with a good yield.  This investment represents extreme security and good income, but, of course, no appreciation. "
                ),

                // SecurityNames.GrowthCorporationOfAmerica
                new Security(
                    name: SecurityNames.GrowthCorporationOfAmerica,
                    fullname: "Growth Corporation Of America",
                    yield: 1,
                    description: "A well established company with a phenomenal growth record.  it is an expanding industry, spends a lot of money on research and is expected to continue its growth pattern.  The Companys policy of reinvesting earnings causes low yield.  The price-to-earnings ratio of this stock is extremely high. "
                ),

                // SecurityNames.MetroPropertiesInc
                new Security(
                    name: SecurityNames.MetroPropertiesInc,
                    fullname: "Metro Properties Inc",
                    yield: 0,
                    description: "An investment representing good prospects of high appreciation.  No dividends are expected in the immediate future.  However, the City’s proposed Urban Renewal Program could have great effect on earnings. "
                ),

                // SecurityNames.PioneerMutualFund
                new Security(
                    name: SecurityNames.PioneerMutualFund,
                    fullname: "Pioneer Mutual Fund",
                    yield: 4,
                    description: "A common stock income mutual fund offered by a large mutual fund company.  It has a good yield of 4%.  Stock in this fund represents a good, steady income, but only a fair appreciation investment."
                ),

                // SecurityNames.ShadyBrooksDevelopment
                new Security(
                    name: SecurityNames.ShadyBrooksDevelopment,
                    fullname: "Shady Brooks Development",
                    yield: 7,
                    description: "A real estate investment representing extremely high income, but steadily depreciating capital assets."
                ),

                // SecurityNames.StrykerDrillingCompany
                new Security(
                    name: SecurityNames.StrykerDrillingCompany,
                    fullname: "Stryker Drilling Company",
                    yield: 0,
                    description: "A stock representing extremely speculative investment.  Profits go toward drilling new oil wells, so no dividends are expected.  This stock could be worth rags or riches."
                ),

                // SecurityNames.TriCityTransportCompany
                new Security(
                    name: SecurityNames.TriCityTransportCompany,
                    fullname: "Tri-City Transport Company",
                    yield: 0,
                    description: "A stock representing a high appreciation investment prospect with a good stable outlook depending on the administrative ability and ambition of its respected, ethical and energetic young president.  As all profits go back into the Company, dividends are not expected to be declared in the foreseeable future."
                ),

                // SecurityNames.UnitedAutoCompany
                new Security(
                    name: SecurityNames.UnitedAutoCompany,
                    fullname: "United Auto Company",
                    yield: 2,
                    description: "A medium large company in the large, oligopolistic automobile industry.  United Auto, like others in the industry, is subject to whims of public fancy.  Stock in this company represents a somewhat speculative investment with a good growth pattern.  Because it is popular with the investing public, it has fairly high price-to-earning ratio and low yield."
                ),

                // SecurityNames.UraniumEnterprisesInc
                new Security(
                    name: SecurityNames.UraniumEnterprisesInc,
                    fullname: "Uranium Enterprises Inc",
                    yield: 6,
                    description: "A highly speculative, high income stock ideal for the short or medium term investor.  Its long term prospects are fair to poor. "
                ),

                // SecurityNames.ValleyPowerAndLightCompany
                new Security(
                    name: SecurityNames.ValleyPowerAndLightCompany,
                    fullname: "Valley Power And Light Company",
                    yield: 3,
                    description: "A stable, steadily growing public utility company located in a well established, healthy industrial area.  Stock in this company represents a safe, medium yield, medium growth investment. "
                )
            };
        #endregion
    }
}
