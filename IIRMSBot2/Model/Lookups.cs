using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IIRMSBot2.Model
{
    public class Lookups
    {
        //<select name = "SourceId" ng-model="encodingData.InfoAccId" class="form-control ng-valid ng-not-empty ng-dirty ng-valid-parse valid ng-touched" aria-required="true" aria-invalid="false">
        //                    <option value = "" > Select Info Accuracy</option>
        //                    <!-- ngRepeat: option in infoacc --><option ng-repeat= "option in infoacc" value= "1" class="ng-binding ng-scope">A1</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="2" class="ng-binding ng-scope">A2</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="3" class="ng-binding ng-scope">A3</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="4" class="ng-binding ng-scope">A4</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="5" class="ng-binding ng-scope">A5</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="6" class="ng-binding ng-scope">A6</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="7" class="ng-binding ng-scope">B1</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="8" class="ng-binding ng-scope">B2</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="9" class="ng-binding ng-scope">B3</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="10" class="ng-binding ng-scope">B4</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="11" class="ng-binding ng-scope">B5</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="12" class="ng-binding ng-scope">B6</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="13" class="ng-binding ng-scope">C1</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="14" class="ng-binding ng-scope">C2</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="15" class="ng-binding ng-scope">C3</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="16" class="ng-binding ng-scope">C4</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="17" class="ng-binding ng-scope">C5</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="18" class="ng-binding ng-scope">C6</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="19" class="ng-binding ng-scope">D1</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="20" class="ng-binding ng-scope">D2</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="21" class="ng-binding ng-scope">D3</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="22" class="ng-binding ng-scope">D4</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="23" class="ng-binding ng-scope">D5</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="24" class="ng-binding ng-scope">D6</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="25" class="ng-binding ng-scope">E1</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="26" class="ng-binding ng-scope">E2</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="27" class="ng-binding ng-scope">E3</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="28" class="ng-binding ng-scope">E4</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="29" class="ng-binding ng-scope">E5</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="30" class="ng-binding ng-scope">E6</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="31" class="ng-binding ng-scope">F1</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="32" class="ng-binding ng-scope">F2</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="33" class="ng-binding ng-scope">F3</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="34" class="ng-binding ng-scope">F4</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="35" class="ng-binding ng-scope">F5</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="36" class="ng-binding ng-scope">F6</option><!-- end ngRepeat: option in infoacc --><option ng-repeat="option in infoacc" value="37" class="ng-binding ng-scope">DOC</option><!-- end ngRepeat: option in infoacc -->
        //                </select>

        public Dictionary<string, string> SecurityClass = new Dictionary<string, string>();
        public Dictionary<string, string> Accuracy { get; set; } = new Dictionary<string, string>();
        public Lookups()
        {
            Accuracy["A1"] = "1";
            Accuracy["A2"] = "2";
            Accuracy["A3"] = "3";
            Accuracy["A4"] = "4";
            Accuracy["A5"] = "5";
            Accuracy["A6"] = "6";
            Accuracy["B1"] = "7";
            Accuracy["B2"] = "8";
            Accuracy["B3"] = "9";
            Accuracy["B4"] = "10";
            Accuracy["B5"] = "11";
            Accuracy["B6"] = "12";
            Accuracy["C1"] = "13";
            Accuracy["C2"] = "14";
            Accuracy["C3"] = "15";
            Accuracy["C4"] = "16";
            Accuracy["C5"] = "17";
            Accuracy["C6"] = "18";
            Accuracy["D1"] = "19";
            Accuracy["D2"] = "20";
            Accuracy["D3"] = "21";
            Accuracy["D4"] = "22";
            Accuracy["D5"] = "23";
            Accuracy["D6"] = "24";
            Accuracy["E1"] = "25";
            Accuracy["E2"] = "26";
            Accuracy["E3"] = "27";
            Accuracy["E4"] = "28";
            Accuracy["E5"] = "29";
            Accuracy["E6"] = "30";
            Accuracy["F1"] = "31";
            Accuracy["F2"] = "32";
            Accuracy["F3"] = "33";
            Accuracy["F4"] = "34";
            Accuracy["F5"] = "35";
            Accuracy["F6"] = "36";
            Accuracy["DOC"] = "37";

            SecurityClass["NONE"] = "0";
            SecurityClass["RESTRICTED"] = "1";
            SecurityClass["CONFIDENTIAL"] = "2";
            SecurityClass["SECRET"] = "3";
            SecurityClass["TOP SECRET"] = "4";


            foreach (var rptType in JsonConvert.DeserializeObject<List<ReportType>>(File.ReadAllText("ReportTypes.json")))
            {
                ReportTypes[rptType.ReportName] = rptType.ReportID;
            }

            foreach (var rptType in JsonConvert.DeserializeObject<List<Office>>(File.ReadAllText("Offices.json")))
            {
                Offices[rptType.OfficeName] = rptType.OfficeId;
            }
        }

        public Dictionary<string, string> ReportTypes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Offices { get; set; } = new Dictionary<string, string>();
    }
}
