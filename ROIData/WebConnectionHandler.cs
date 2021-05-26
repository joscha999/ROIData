using Newtonsoft.Json;
using Planspiel.Models;
using ProjectAutomata;
using ROIData.Models;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ROIData
{
    public static class WebConnectionHandler
	{
		private const string baseAddress = "https://roi.jgdev.de/api/";
		//private const string baseAddress = "http://192.168.178.23:5001/api/";
		private const string postAddress = baseAddress + "Data";
		private const string getAddress = baseAddress + "TaskAPI";
		private static string sdpath = System.IO.Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RiseOfIndustry", "SaveData");
		private static float RealTimeSinceLastUpdate;

		public static void Update(ROIDataMod mod)
        {
			RealTimeSinceLastUpdate += Time.unscaledDeltaTime;
            if (RealTimeSinceLastUpdate > 10)
            {
				RealTimeSinceLastUpdate = 0;
				mod.StartCoroutine(GetRequest(getAddress + "?groupSteamID="
					+ (long)SteamUser.GetSteamID().m_SteamID, TaskSystem.ReceiveTasks));
			}
		}

		public static void SendData(ROIDataMod mod)
		{
			SaveDataModel sdm = new SaveDataModel
			{
				SteamID = (long)SteamUser.GetSteamID().m_SteamID,
				UnixDays = TimeStampCalculator.GetTimeStamp().UnixDays,
				PassedTime = Time.realtimeSinceStartup,
				Profit = PlayerProfitCalculator.GetProfit(),
				CompanyValue = CompanyValueCalculator.GetCompanyValue(),
				DemandSatisfaction = DemandSatisfactionCalculator.RemainingDemands(),
				MachineUptime = MachineUptimeCalculator.GetAverageMachineUptime(),
				LoansList = LoanCalculator.GetLoansList(),
				AveragePollution = PollutionCalculator.GetAveragePollution(),
				BuildingCount = BuildingCountCalculator.GetBuildingCount(),
				RegionCount = RegionCountCalculator.GetRegionsCount(),
				UnlockedResearchCount = TechTreeCalculator.GetTechTreeUnlocks(),
				Balance = PlayerBalance.GetPlayerBalance()
			};

			var jsonData = JsonConvert.SerializeObject(sdm);
			Directory.CreateDirectory(sdpath);

			try
			{
				File.WriteAllText(System.IO.Path.Combine(
					sdpath, sdm.UnixDays.ToString() + ".json"),
					jsonData);
			}
			catch (Exception e)
			{
				Debug.Log("Failed to serialize:\n" + e);
				throw;
			}

			Debug.Log(jsonData);
			//TechTreeCalculator.PrintInformation();
			//PollutionCalculator.GetAveragePollutionAdv();
			mod.StartCoroutine(PostRequest(postAddress, jsonData));
		}

		private static IEnumerator PostRequest(string url, string json)
		{
			var uwr = new UnityWebRequest(url, "POST");
			byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
			uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
			uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");

			//Send the request then wait here until it returns
			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError)
			{
				Debug.Log("POST: Error While Sending: " + uwr.error);
			}
			else
			{
				Debug.Log("POST: Received: " + uwr.downloadHandler.text);
			}
		}

		private static IEnumerator GetRequest(string url, Action<List<AssignmentTask>> callBack)
		{
			UnityWebRequest uwr = UnityWebRequest.Get(url);
			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError)
			{
				Debug.Log("GET: Error While Sending: " + uwr.error);
			}
			else
			{
				Debug.Log("GET: Received: " + uwr.downloadHandler.text);
				callBack?.Invoke(JsonConvert.DeserializeObject<List<AssignmentTask>>(uwr.downloadHandler.text));
			}
		}
	}
}
