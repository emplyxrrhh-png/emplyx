import * as baseHelper from "../../../helpers/baseHelper"
import * as loginHelper from "../../../helpers/loginHelper"
import * as apiHelper from "../../../helpers/apiHelper"
import { result } from "cypress/types/lodash";


describe.skip('Live Login', () => {

//////////////////////////
// Se debe limpiar las cookies antes de realizar las pruebas.
  beforeEach(() => {
    cy.visit('https://vtliveidi.azurewebsites.net/LoginWeb.aspx')

    cy.window().then((win) => {
      try {
        win.logoutClient();
      } catch (e) {
        cy.log(e)
      }
    });

  })
//////////////////////////
// Siempre se debe limpiar la base de datos después de realizar las pruebas.
  afterEach(() => {

    apiHelper.LiveResertLogin("9");
    cy.sqlServer("UPDATE sysroPassports_AuthenticationMethods SET InvalidAccessAttemps=0,BloquedAccessApp=0  WHERE IDPassport in (161,331) AND Method=1").then((result) => {
    })
  
  })

  it('Positivo', () => {
    cy.get('#ServerText').clear().type('cypr6036')
    cy.get('#UserName').clear().type('g.loco')
    cy.get('#Password').clear().type('12345678As+')
    cy.get('#btAccept').click()

    cy.url().should('include', '/Main.aspx')

    cy.window().then((win) => {
      win.logoutClient();
    });
    cy.url().should('include', '/LoginWeb.aspx')
  })

  it('Contraseña erronia', () => {
    cy.sqlServer("UPDATE sysroPassports_AuthenticationMethods SET InvalidAccessAttemps=0  WHERE IDPassport=161 AND Method=1").then((result) => {
    })
    apiHelper.LiveResertLogin("161");
    cy.get('#ServerText').clear().type('cypr6036')
    cy.get('#UserName').clear().type('g.loco')
    cy.get('#Password').clear().type('1234578As+ 66')
    cy.get('#btAccept').click()

    cy.get("#lblResult").invoke('text').then((text) => {
      expect(text).to.equal("Identificación incorrecta")
    })

    const result = apiHelper.InvalidAccessAttempsByIDPassport("161");
    expect(result).to.equal(1);

    cy.sqlServer("SELECT InvalidAccessAttemps FROM sysroPassports_AuthenticationMethods WHERE IDPassport=161 AND Method=1").then((result) => {
      expect(result).to.equal(1)
    })



  })

  it('Cliente inexistente', () => {
    cy.get('#ServerText').clear().type('Supercompany')
    cy.get('#UserName').clear().type('g.loco')
    cy.get('#Password').clear().type('1234578As+')
    cy.get('#btAccept').click()

    cy.get("#lblResult").contains("Unknown application error")

  })
  it('Bloqueo temporal', () => {
    cy.sqlServer("UPDATE sysroPassports_AuthenticationMethods SET InvalidAccessAttemps=5  WHERE IDPassport=161 AND Method=1").then((result) => {
    })
    cy.get('#ServerText').clear().type('cypr6036')
    cy.get('#UserName').clear().type('g.loco')
    cy.get('#Password').clear().type('1234578As+ 66')
    cy.get('#btAccept').click()

    cy.get("#lblResult").invoke('text').then((text) => {
      expect(text).to.equal("Acceso bloqueado temporalmente")
    })
    cy.sqlServer("SELECT InvalidAccessAttemps FROM sysroPassports_AuthenticationMethods WHERE IDPassport=161 AND Method=1").then((result) => {
      expect(result).to.equal(6)
    })
  })

  it('Bloqueo permanente', () => {
    cy.sqlServer("UPDATE sysroPassports_AuthenticationMethods SET InvalidAccessAttemps=10  WHERE IDPassport=161 AND Method=1").then((result) => {
    })
    cy.get('#ServerText').clear().type('cypr6036')
    cy.get('#UserName').clear().type('g.loco')
    cy.get('#Password').clear().type('1234578As+ 66')
    cy.get('#btAccept').click()

    cy.get("#lblResult").invoke('text').then((text) => {
      expect(text).to.equal("Cuenta bloqueada")
    })

    cy.sqlServer("SELECT InvalidAccessAttemps,BloquedAccessApp FROM sysroPassports_AuthenticationMethods WHERE IDPassport=161 AND Method=1").then((result) => {
      expect(result[0]).to.equal(0)
      expect(result[1]).to.equal(true)
    })
  })

  it('Supervisor inctivo', () => {
    cy.get('#ServerText').clear().type('cypr6036')
    cy.get('#UserName').clear().type('Silvia')
    cy.get('#Password').clear().type('1234As')
    cy.get('#btAccept').click()
    cy.get("#lblResult").invoke('text').then((text) => {
      expect(text).to.equal("Usuario inactivo")
    })
  })

})