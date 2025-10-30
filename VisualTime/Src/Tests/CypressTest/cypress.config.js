const { defineConfig } = require("cypress");
const sqlServer = require("cypress-sql-server");
const { t } = require("i18next-ko");
const { addMatchImageSnapshotPlugin } = require('@simonsmith/cypress-image-snapshot/plugin');
const installLogsPrinter = require("cypress-terminal-report/src/installLogsPrinter");
const cypressFailFast = require("cypress-fail-fast/plugin");

module.exports = defineConfig({
  experimentalWebKitSupport: true,
  screenshotOnRunFailure: false,
  chromeWebSecurity: false,
  reporter: 'cypress-multi-reporters',
  reporterOptions: {
    configFile: 'reporter-config.json',
  },

  env: {
    db: {
      server: "10.9.0.4",
      userName: "saDev",
      password: "VisualTime#1",
      options: {
        database: "devDev_cypress",
        trustedConnection: true,
      },
    },
    DB_SERVER: "10.9.0.4",
    DB_PORT: 1433,
    DB_NAME: "devDev_cypress",
    DB_USER: "saDev",
    DB_PASSWORD: "VisualTime#1",
    
    BASE_API_URL: "https://vtliveidi.azurewebsites.net", // URL base para las APIs
    DEFAULT_TIMEOUT: 30000, // Tiempo de espera predeterminado
    ENABLE_DEBUG: false, // Habilita o deshabilita el modo de depuración

    FAIL_FAST_STRATEGY: 'run', // Estrategia de fallo rápido (spec o suite)
    FAIL_FAST_ENABLED: false, // Habilita o deshabilita el fallo rápido
    FAIL_FAST_BAIL: 10, // Número de fallos permitidos antes de detener la ejecución
    FAIL_FAST_PLUGIN: true, // Habilita o deshabilita el plugin de fallo rápido
    FAIL_FAST_LOG: true, // Habilita o deshabilita el registro de fallos

  },



  e2e: {
    setupNodeEvents(on, config) {
      cypressFailFast(on, config);
      installLogsPrinter(on, {
        printLogsToConsole: "always", // Siempre imprime los logs
        includeSuccessfulHookLogs: true, // Incluye logs de hooks como beforeEach y afterEach
       debug: false, // Activa el modo de depuración para obtener más información
       outputVerbose: true, // Imprime logs detallados
       
      });
      addMatchImageSnapshotPlugin(on);

      // allows db data to be accessed in tests
      config.db = {
        "userName": "saDev",
        "password": "VisualTime#1",
        "server": "10.9.0.4",
        "options": {
          "database": "devDev_cypress",
          "encrypt": true,
          "rowCollectionOnRequestCompletion": true
        }
      }
      // code from /plugins/index.js
      const tasks = sqlServer.loadDBPlugin(config.db);
      on('task', tasks);
      return config
    },
    baseUrl: "https://vtliveidi.azurewebsites.net",

    defaultCommandTimeout: 30000,
    viewportWidth: 1280,
    viewportHeight: 720,
    requestTimeout: 70000,
    downloadsFolder: "cypress/downloads",
    testIsolation: true,
    retries: {
      runMode: 2, // Reintenta 2 veces en modo headless
      openMode: 0, // No reintenta en modo interactivo
    },
  },
});
