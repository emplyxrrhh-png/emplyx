

import * as loginHelper from "../../../helpers/loginHelper"

describe('Expediente RRHH-', () => {

  beforeEach(() => {
    loginHelper.login('g.loco', '12345678As+')
    cy.intercept('POST', ' https://vtliveidi.azurewebsites.net/Cookie/GetCookie').as('GetCookie');
       
    cy.window().then((win) => {
      //win.reenviaFrame('/./Employees/Employees.aspx', '', 'Expediente de RRHH', 'PortalUsersEmployees');
      win.reenviaFrame('/./Employees/Employees.aspx', '', '', '');
    });
    cy.wait('@GetCookie').its('response.statusCode').should('eq', 200);
    cy.wait(1000 * 5)
    

  });

  afterEach(() => {
    loginHelper.logout()
  })



  it('Test para verificar que el usuario pueda ingresar al sistema y que los Iframes estén cargados accediendo a la sección de empleados.', () => {
    
    cy.getIframeBody('#ifPrincipal').find('#TABBUTTON_UserFields').invoke('text').then((text) => {
      expect(text).to.equal("Usuarios")
    })

  })

  //////////////////////////
  // 3. Test para acceder y ver documentos del empleado en el sistema.
  it.skip('Test para acceder y ver documentos del empleado en el sistema.', () => {
    cy.clearAllCookies();
    cy.visit('https://vtliveidi.azurewebsites.net/LoginWeb.aspx')
    cy.get('#ServerText').clear()
    cy.get('#ServerText').type('dev3')
    cy.get('#UserName').clear()
    cy.get('#UserName').type('g.loco')
    cy.get('#Password').type('12345678As+')
    cy.get('#btAccept').click()

    cy.get('#ifPrincipal').within(() => {
      cy.get('.html')

    })

  })
  //////////////////////////
  // 4. Test para acceder y ver documentos del empleado en el sistema.
  it.skip('DocumentosAlert - SIN TERMINAR', () => {
    //loginHelper.login('g.loco', '12345678As+')
    cy.clearAllCookies();
    cy.visit('https://vtliveidi.azurewebsites.net/LoginWeb.aspx')
    cy.get('#ServerText').clear()
    cy.get('#ServerText').type('dev3')
    cy.get('#UserName').clear()
    cy.get('#UserName').type('g.loco')
    cy.get('#Password').type('12345678As+')
    cy.get('#btAccept').click()


    cy.get('#MainMenu_btnAlerts').click()

    cy.pause()

    cy.get("#ctl00_contentMainBody_ASPxCallbackPanelContenido_userAlertsContent > div:nth-child(4)").click()

    cy.pause()


    cy.log("Paso 1")

  })
  //////////////////////////
  // 5. Test para acceder y ver documentos del empleado en el sistema directamente por URL.
  it.skip('DocumentosAlertDirect - SIN TERMINAR', () => {
    cy.clearAllCookies();
    cy.visit('https://vtliveidi.azurewebsites.net/LoginWeb.aspx')
    cy.get('#ServerText').clear()
    cy.get('#ServerText').type('dev3')
    cy.get('#UserName').clear()
    cy.get('#UserName').type('g.loco')
    cy.get('#Password').type('12345678As+')
    cy.get('#btAccept').click()
    cy.pause()
    cy.visit('https://vtliveidi.azurewebsites.net/Alerts/AlertsDetail.aspx?NotificationType=-1&DocumentAlertType=0&DocumentType=EMPLOYEE&IdRelatedObject=-1')
  })

  //////////////////////////
  // 6. Test para crear un empleado
  it.skip('Crear un empleado - (NoTerminado)', () => { //Task 1713199: Cambiar Whizard  top.ShowExternalForm2
    cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Employees/Scripts/Employees_v2.js*').as('Employees_v2');
    cy.wait('@Employees_v2').its('response.statusCode').should('eq', 200);
    cy.getIframeBody('#ifPrincipal').find('#ctl00_contentMainBody_roTrees1_spanCaption').invoke('text').then((text) => {
      expect(text).to.equal("Usuarios")
    })

  

     
   /* 
     cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Employees/Scripts/Employees_v2.js*').as('Employees_v2');
     //cy.wait('@Employees_v2').its('response.statusCode').should('eq', 200);
 
     cy.wait('@Employees_v2').then((interception) => {
      cy.log(interception.response.body);
      cy.log(interception.response.statusCode.toString());
     }) */

    cy.getIframeBody('#ifPrincipal').find('#aspnetForm').as('main')
    cy.wait(1000 * 7);

   /*  onBeforeLoad: (win) => {
    cy.window().then((win) => {
      win.ShowNewEmployeeWizard = function () {
        if (actualGrupo > 0) {
          var Title = '';
          var url = 'Employees/Wizards/NewMultiEmployeeWizard.aspx?GroupID=' + actualGrupo;
          top.frames[0].ShowExternalForm2(url, 800, 520, Title, '', false, false, false);
          LoadEmployeeGridWizard();
        } else {
          showErrorPopup("Error.SelectionTitle", "error", "Error.MustSelectAGroup", "Error.OK", "Error.OKDesc", "");
        }
      };
    });
  }); */

   
    cy.get('@main').find('#divBarButtons > div > div > a.ButtonBarVertical.btnTbAddEmp2').trigger('mouseover').click()


    // win.ShowNewEmployeeWizard()
    cy.wait(1000 * 3);
    cy.pause();
  })


})

