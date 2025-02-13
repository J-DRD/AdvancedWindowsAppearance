﻿using Octokit;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
	public class Updater
	{
		private IReleasesClient _releaseClient;
		private GitHubClient Github;
		private readonly string RepositoryOwner = "ArkadySK";
		private readonly string RepositoryName = "AdvancedWindowsAppearance";
		private readonly string PackageName = "AWA"; //the downloading package
		private Version CurrentVersion;
		private Version LatestVersion;

		public Updater()
		{
			CurrentVersion = GetCurrentVersionInfo();

			Github = new GitHubClient(new ProductHeaderValue(RepositoryName + @"-UpdateCheck"));
			_releaseClient = Github.Repository.Release;
		}

		public Version GetCurrentVersionInfo()
		{
			return Assembly.GetExecutingAssembly().GetName().Version;
		}

		public async Task<bool> IsUpToDate()
		{
			Version newVersion = await GetNewVersionInfoAsync();

			return (newVersion == CurrentVersion);
		}

		#region get new version name

		private Version StringToVersion(string versionString)
		{
			versionString = versionString.Replace("v", string.Empty);
			try
			{
				return new Version(versionString);
			}
			catch
			{
				versionString = versionString.Replace("-beta", string.Empty);
				versionString = versionString.Replace("-alpha", string.Empty);
				return new Version(versionString);
			}
		}

		public async Task<Version> GetNewVersionInfoAsync()
		{
			if (String.IsNullOrWhiteSpace(RepositoryName) || String.IsNullOrWhiteSpace(RepositoryOwner)) return null;

			var allReleases = await _releaseClient.GetAll(RepositoryOwner, RepositoryName);
			var latestRelease = allReleases.FirstOrDefault(release => StringToVersion(release.TagName) > CurrentVersion);
			if (latestRelease != null)
				LatestVersion = StringToVersion(latestRelease.TagName);
			else
				LatestVersion = CurrentVersion;
			return LatestVersion;
		}

		#endregion get new version name

		#region Download

		public void DownloadUpdate()
		{
			const string urlTemplate = "https://github.com/{0}/{1}/releases/download/{2}/{3}";
			var url = string.Format(urlTemplate, RepositoryOwner, RepositoryName, "v" + LatestVersion, PackageName + ".zip");

			url = url.Replace("&", "^&");
			Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
		}

		#endregion Download
	}
}
