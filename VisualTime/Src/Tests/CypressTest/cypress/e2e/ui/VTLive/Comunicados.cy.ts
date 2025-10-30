import * as baseHelper from "../../../helpers/baseHelper"
import * as loginHelper from "../../../helpers/loginHelper"

describe('Comunicados-', () => {
  function waitForIframeToLoad(iframeSelector) {
    cy.get(iframeSelector).then($iframe => {
      const iframe = $iframe[0];
      return new Cypress.Promise(resolve => {
        iframe.onload = () => {
          resolve();
        };
      });
    });
  }


  beforeEach(() => {


    loginHelper.login('g.loco', '12345678As+')
    cy.window().then((win) => {
      win.reenviaFrame('/Communique.aspx', '', '', '');
    })
    cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Communique/GetAllCommuniques').as('getCommuniques');

  });

  afterEach(() => {
    loginHelper.logout()
  })

  it('Existe página comunicados y tengo permisos para acceder', () => {
    cy.wait('@getCommuniques').its('response.statusCode').should('eq', 200);
  });
  it('Realizo una carga completa de la página y carga correctamente', () => {


    cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Communique/GetPermisionOverCommuniques').as('getPermisionOverCommuniques');
    cy.wait('@getPermisionOverCommuniques').its('response.statusCode').should('eq', 200);
    // Check if the title is correct
    cy.getIframeBody('#ifPrincipal').find('#main').as('main')
    cy.get('@main').find('#ctl00_contentMainBody_lblHeader').invoke('text').then((text) => {
      expect(text).to.equal("Comunicación Interna")
    })

  });


  it('Se crea un nuevo comunicado con Asunto', () => {
    cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Communique/GetPermisionOverCommuniques').as('getPermisionOverCommuniques');
    cy.wait('@getPermisionOverCommuniques').its('response.statusCode').should('eq', 200);

    cy.wait('@getCommuniques').its('response.statusCode').should('eq', 200);
    cy.getIframeBody('#ifPrincipal').find('#main').as('main')

    cy.url().should('include', '/Communique');
    cy.get('@main').find('#mainPanelDisplay').should('be.visible')


    cy.wait(1000 * 3)
    cy.get('@main').find('#commCreateBtn').click()


    cy.get('@main').find('#configCommunique')
    cy.wait(1000 * 3)
    cy.get('@main').find('input[name="Asunto"]').type("Comunicado de prueba")

    cy.wait(1000 * 3)
    cy.get('@main').find('#communiqueeSave').click()

    cy.get('@main').find('#mainPanelDisplay').should('be.visible')
    cy.get('@main').find('#CardView_tccv0 > div.communiqueCardInfo > h3').invoke('text').should('eq', 'Comunicado de prueba')

  });

  it('Se elimina un nuevo comunicado con Asunto', () => {
    cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Communique/GetPermisionOverCommuniques').as('getPermisionOverCommuniques');
    cy.wait('@getPermisionOverCommuniques').its('response.statusCode').should('eq', 200);

    cy.wait('@getCommuniques').its('response.statusCode').should('eq', 200);
    cy.getIframeBody('#ifPrincipal').find('#main').as('main')

    cy.url().should('include', '/Communique');
    cy.get('@main').find('#mainPanelDisplay').should('be.visible')
    cy.get('@main').find('#configCommunique')
    cy.get('@main').find('#CardView_tccv0 > div.communiqueCardInfo > h3').invoke('text').should('eq', 'Comunicado de prueba')
    cy.wait(1000 * 3)


    cy.get('@main').find('#CardView_tccv0 > div.communiqueCardInfo').invoke('attr', 'data-communique-id').then((communiqueId) => {

      cy.log(communiqueId)

      cy.request({
        method: 'POST',
        url: 'https://vtliveidi.azurewebsites.net/Communique/DeleteCommunique',
        body: {
          "IdCommunique": communiqueId
        }
      }
      ).then((response) => {
        // TODO: Check if the response is correct
        // Verifica que el estado de la respuesta sea 200
        expect(response.status).to.eq(200);
        expect(response.body).to.eq('True');
      });

    });

  })

  it('Nuevo comunicado - Asunto+Diseño', () => {

    cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Communique/GetPermisionOverCommuniques').as('getPermisionOverCommuniques');
    cy.wait('@getPermisionOverCommuniques').its('response.statusCode').should('eq', 200);

    cy.wait('@getCommuniques').its('response.statusCode').should('eq', 200);
    cy.getIframeBody('#ifPrincipal').find('#main').as('main')

    cy.url().should('include', '/Communique');
    cy.get('@main').find('#mainPanelDisplay').should('be.visible')

    cy.wait(1000 * 3)
    cy.get('@main').find('#commCreateBtn').click()

    cy.get('@main').find('#configCommunique')
    cy.wait(1000 * 3)
    cy.get('@main').find('input[name="Asunto"]').type("Comunicado de prueba Diseño")

    cy.get('@main').find('#communiqueeDesign').click()

    cy.get('@main').find('#messageEditor > div.dx-quill-container.ql-container > div.ql-editor').type("Comunicado de prueba Diseño")

    cy.wait(1000 * 3)
    cy.get('@main').find('#communiqueeSave').click()

    cy.get('@main').find('#mainPanelDisplay').should('be.visible')
    cy.get('@main').find('#CardView_tccv0 > div.communiqueCardInfo > h3').invoke('text').should('eq', 'Comunicado de prueba Diseño')

    cy.get('@main').find('#CardView_tccv0 > div.communiqueCardInfo').invoke('attr', 'data-communique-id').then((communiqueId) => {
      cy.log(communiqueId)
      cy.request({
        method: 'POST',
        url: 'https://vtliveidi.azurewebsites.net/Communique/DeleteCommunique',
        body: {
          "IdCommunique": communiqueId
        }
      }
      ).then((response) => {
        // TODO: Check if the response is correct
        // Verifica que el estado de la respuesta sea 200
        expect(response.status).to.eq(200);
        expect(response.body).to.eq('True');
      });

    });

  })

  it.skip('Nuevo comunicado - Seleccion basica Empleado- (NoTerminado)', () => { 

    cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Communique/GetPermisionOverCommuniques').as('getPermisionOverCommuniques');
    cy.wait('@getPermisionOverCommuniques').its('response.statusCode').should('eq', 200);

    cy.wait('@getCommuniques').its('response.statusCode').should('eq', 200);
    cy.getIframeBody('#ifPrincipal').find('#main').as('main')

    cy.url().should('include', '/Communique');
    cy.get('@main').find('#mainPanelDisplay').should('be.visible')

    cy.wait(1000 * 3)
    cy.get('@main').find('#commCreateBtn').click()

    cy.get('@main').find('#configCommunique')
    cy.wait(1000 * 3)
    cy.get('@main').find('input[name="Asunto"]').type("Comunicado de prueba Diseño")

    cy.get('@main').find('#communiqueeDesign').click()

    cy.get('@main').find('#messageEditor > div.dx-quill-container.ql-container > div.ql-editor').type("Comunicado de prueba Diseño")




    cy.get('@main').find('#communiqueeConfiguration').click()
    cy.get('@main').find('.CY-btnSelectDest').click()
    cy.wait(1000 * 1)

    
    cy.getIframeBody('#ifPrincipal').find('#EmployeeText > div.dx-dropdowneditor-input-wrapper.dx-selectbox-container > div > div.dx-texteditor-input-container.dx-tag-container').click()


    document.querySelector("#dx-e1e34b24-03ee-00a3-e1e3-1bcfdd8b8d54 > div.dx-scrollable-wrapper > div > div.dx-scrollable-content > div.dx-scrollview-content > div.dx-list-select-all > div.dx-widget.dx-checkbox.dx-list-select-all-checkbox > input[type=hidden]")

cy.pause()

  })

  it.skip('Nuevo comunicado - Asunto+Diseño+Empleado- (NoTerminado)', () => { // Task 1682186: Selector "simple" de empleados || Cambiar ID de los selects
    cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Communique/GetPermisionOverCommuniques').as('getPermisionOverCommuniques');
    cy.wait('@getPermisionOverCommuniques').its('response.statusCode').should('eq', 200);

    cy.wait('@getCommuniques').its('response.statusCode').should('eq', 200);
    cy.getIframeBody('#ifPrincipal').find('#main').as('main')

    cy.url().should('include', '/Communique');
    cy.get('@main').find('#mainPanelDisplay').should('be.visible')

    cy.wait(1000 * 3)
    cy.get('@main').find('#commCreateBtn').click()

    cy.get('@main').find('#configCommunique')
    cy.wait(1000 * 3)
    cy.get('@main').find('input[name="Asunto"]').type("Comunicado de prueba Diseño")

    cy.get('@main').find('#communiqueeDesign').click()

    cy.get('@main').find('#messageEditor > div.dx-quill-container.ql-container > div.ql-editor').type("Comunicado de prueba Diseño")




    cy.get('@main').find('#communiqueeConfiguration').click()




    cy.get('@main').find('.CY-btnSelectDest').click()
    cy.wait(1000 * 1)
    cy.getIframeBody('#ifPrincipal').find('#showAdvanced').click()
    cy.wait(1000 * 1)


    cy.getIframeBody2iframe('#ifPrincipal', '#ifEmployeesSelector').as('EmployeesSelector')
    cy.wait(1000 * 1)


    cy.get('@EmployeesSelector').find('#form1')

    cy.get('@EmployeesSelector').find("#ext-gen7")
    cy.get('@EmployeesSelector').find("#ext-gen7").find("[ext:tree-node-id='A2']")



    // TODO: Select the first employee.

    cy.wait(1000 * 3)
    cy.get('@main').find('#communiqueeSave').click()

    cy.get('@main').find('#mainPanelDisplay').should('be.visible')
    cy.get('@main').find('#CardView_tccv0 > div.communiqueCardInfo > h3').invoke('text').should('eq', 'Comunicado de prueba Diseño')

    cy.get('@main').find('#CardView_tccv0 > div.communiqueCardInfo').invoke('attr', 'data-communique-id').then((communiqueId) => {
      cy.log(communiqueId)
      cy.request({
        method: 'POST',
        url: 'https://vtliveidi.azurewebsites.net/Communique/DeleteCommunique',
        body: {
          "IdCommunique": communiqueId
        }
      }
      ).then((response) => {

        expect(response.status).to.eq(200);
        expect(response.body).to.eq('True');
      });

    });
  })
  it.skip('Nuevo comunicado - Asunto+Diseño+Empleado+Ennvio - (NoTerminado)', () => { // Task 1682186: Selector "simple" de empleados || Cambiar ID de los selects
  })

});