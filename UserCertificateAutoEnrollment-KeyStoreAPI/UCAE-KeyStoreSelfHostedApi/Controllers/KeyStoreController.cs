﻿using Microsoft.AspNetCore.Mvc;
using UserCertificateAutoEnrollment.BL.KeyStore;
using UserCertificateAutoEnrollment.BL.Session;

namespace UCAE_KeyStoreSelfHostedApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KeyStoreController : Controller
    {
        private readonly IKeyStoreFactory m_KeyStoreFactory;
        private readonly ISessionProvider m_SessionProvider;
        private readonly NLog.Logger m_Logger = NLog.LogManager.GetCurrentClassLogger();

        public KeyStoreController(IKeyStoreFactory keyStoreFactory, ISessionProvider sessionProvider)
        {
            m_KeyStoreFactory = keyStoreFactory;
            m_SessionProvider = sessionProvider;
        }

        [HttpGet]
        [Route("GetCertificates")]
        public async Task<IActionResult> GetCertificatesAsync(string ssTType)
        {
            m_Logger.Info($"List of certificates was requested");            
            var os = "windows";
            m_Logger.Info($"Retriving list of certificates from {os} OS");
            var keyStoreManager = m_KeyStoreFactory.GetKeyStoreManager(os);
            m_Logger.Trace($"Created key store manager of type {keyStoreManager.KeyStoreResolver.GetType()}");

            var certificates = await keyStoreManager.GetCertificatesAsync(ssTType);
            m_Logger.Info("Certificates retrieved successfully");
            m_Logger.Trace($"Number of certificates retrieved from {os} key store {certificates.Count()}");

            //return new List<object>();

            return Ok(certificates);
        }

        [HttpPost]
        public async Task<IActionResult> SyncCertificate(string ssTType)
        {
            m_Logger.Info("Syncing certificates with Allianz servers");
            m_Logger.Trace($"Sync certificates for {ssTType} was called...");

            var os = "windows";

            m_Logger.Info($"Sync started for {os} OS");
            var keyStore = m_KeyStoreFactory.GetKeyStoreManager(os);
            m_Logger.Trace($"Created key store manager of type {keyStore.GetType()}");

            await keyStore.SyncCertificatesAsync(ssTType);

            return Ok();
        }
    }
}
