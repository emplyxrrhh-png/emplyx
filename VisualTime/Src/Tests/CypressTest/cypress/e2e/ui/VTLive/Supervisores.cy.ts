
import * as loginHelper from "../../../helpers/loginHelper"

describe('Supervisores', () => {

  beforeEach(() => {
    loginHelper.login('g.loco', '12345678As+')
    cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Security/SupervisorsContent.aspx').as('SupervisorsContent');
    cy.window().then((win) => {
      win.reenviaFrame('/.//Supervisors', 'MainMenu_Organización_MainMenu_Button2', 'Supervisores', 'Portal\\Company');
    });

    cy.wait('@SupervisorsContent').its('response.statusCode').should('eq', 200);
    cy.wait(1000 * 5)


  });

  afterEach(() => {
    loginHelper.logout()
  })




  it('Test para verificar que el usuario pueda ingresar al sistema y que los Iframes estén cargados accediendo a la sección de Supervisores.', () => {

    cy.getIframeBody('#ifPrincipal').find('#ctl00_contentMainBody_lblHeader').invoke('text').then((text) => {
      expect(text).to.equal("admin")
    })

  })


  it('Test para seleccionar un supervisor', () => {

    
    cy.getIframeBody('#ifPrincipal').find('#main').as('main')
    cy.get('@main').find('[data-card-id="20"] > h3 ').invoke('text').then((text) => {
      expect(text).to.equal("supervisorempleadoprueba")
    })
    cy.get('@main').find('[data-card-id="20"] > h3 ').click()

    cy.get('@main').find('#ctl00_contentMainBody_lblHeader').invoke('text').then((text) => {
      expect(text).to.equal("supervisorempleadoprueba")
    })
  })

  it.skip('Test para crear un supervisor - NoTerminado', () => { //Task 1713199: Cambiar Whizard  top.ShowExternalForm2

    cy.wait(1000 * 5)
    cy.getIframeBody('#ifPrincipal').find('#main').as('main')
    cy.get('@main').find('#divBarButtons > div > div > div:nth-child(3) > a').click()
    cy.pause()
  })

})