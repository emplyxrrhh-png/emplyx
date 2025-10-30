// ***********************************************************
// This example support/e2e.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands';
//import "cypress-fail-fast";



require("cypress-fail-fast"); // Importar el plugin de cypress-fail-fast



// Alternatively you can use CommonJS syntax:
// require('./commands')
// Options for log collector
const options = {
  // Log console output only
  collectTypes: ['cons:log', 'cons:info', 'cons:warn', 'cons:error','cy:log'],
  enableExtendedCollector: true, // Enable extended collector for more detailed logs
  enableContinuousLogging: true, // Log to files after each run
}
require('cypress-terminal-report/src/installLogsCollector')(options);

//import sqlServer from 'cypress-sql-server';
import {addMatchImageSnapshotCommand} from '@simonsmith/cypress-image-snapshot/command'

//sqlServer.loadDBCommands(); 

addMatchImageSnapshotCommand()

// can also add any default options to be used
// by all instances of `matchImageSnapshot`
addMatchImageSnapshotCommand({
   // options for jest-image-snapshot
   failureThreshold: 0.2,
   comparisonMethod: 'ssim',
   snapFilenameExtension: '.snap',
  diffFilenameExtension: '.diff',
})

Cypress.on('fail', (error, runnable) => {
  cy.log('Error capturado:', error.message);
  return false; // Evita que Cypress detenga la ejecución
});