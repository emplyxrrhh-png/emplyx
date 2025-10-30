
export function login(user: string, password: string): Promise<boolean> {
    cy.log('Iniciando inicio de sesión...');
    // Intercepta todas las peticiones necesarias
    cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Start').as('getStart');
    cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Start/GetEmployees?*').as('GetEmployees');
    cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Start/GetAlerts?*').as('GetAlerts');
    cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Start/GetRequests?*').as('GetRequests');
    cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Start/GetTelecommutingResume?*').as('GetTelecommutingResume');
    cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Start/GetAbsenceResume?*').as('GetAbsenceResume');
    cy.intercept('GET', 'https://vtliveidi.azurewebsites.net/Start/GetEmployeeResume?*').as('GetEmployeeResume');


    cy.clearAllCookies();
    cy.visit('https://vtliveidi.azurewebsites.net/LoginWeb.aspx')
    cy.get('#ServerText').clear().type('cypr6036')
    cy.get('#UserName').clear().type(user)
    cy.get('#Password').clear().type(password)
    cy.get('#btAccept').click()

    cy.url().should('include', '/Main.aspx');



    cy.wait(1000 * 3)
    // Espera a que todas las peticiones respondan
    cy.wait('@getStart').its('response.statusCode').should('eq', 200);
    cy.wait('@GetEmployees').its('response.statusCode').should('eq', 200);
    cy.wait('@GetAlerts').its('response.statusCode').should('eq', 200);
    cy.wait('@GetRequests').its('response.statusCode').should('eq', 200);
    cy.wait('@GetTelecommutingResume').its('response.statusCode').should('eq', 200);
    cy.wait('@GetAbsenceResume').its('response.statusCode').should('eq', 200);
    cy.wait('@GetEmployeeResume').its('response.statusCode').should('eq', 200);

    cy.log('Login completado exitosamente.');
    cy.wait(1000 * 2)

    return true
}


export function logout(): Promise<boolean> {
    try {
        cy.log('Iniciando cierre de sesión...');
        cy.window().then((win) => {
            win.logoutClient();
        });
        cy.wait(1000 * 3)
        cy.url().should('include', '/LoginWeb.aspx')
        cy.url().then((url) => {
            try {
              expect(url).to.include('/LoginWeb.aspx');
            } catch (error) {
              cy.log(`Error: La URL actual (${url}) no incluye '/LoginWeb.aspx'`);
            }
          });
        cy.log('Logout completado exitosamente.');
        return true;
    } catch (error) {
        cy.log('Error durante el logout:', error.message);
        return true;
    }
}




export function loginPortal(user: string, password: string): Promise<boolean> {
    cy.intercept('GET', ' https://vtportalidi.azurewebsites.net/api/Portalsvcx.svc/isAdfsActive?*').as('isAdfsActive');
    cy.clearAllCookies();
    cy.visit("https://vtportalidi.azurewebsites.net/2/indexv2.aspx")
    cy.wait(1000 * 4)

 try {
   // cy.wrap('@isAdfsActive').its('response.statusCode').should('eq', 200);
 }catch (error) {
    cy.log('Error al interceptar la petición isAdfsActive:', error.message);
 }
 cy.log('Verificando si el elemento existe...');


 cy.get('body').then(($body) => {
        if ($body.find('.dx-item-content > .dx-button > .dx-button-content > .dx-button-text').length > 0) {
            cy.get('.dx-item-content > .dx-button > .dx-button-content > .dx-button-text').click()
        } else {
            cy.log('El elemento no existe');
        }
    });

    cy.get('div[data-bind="dxTextBox: txtClient, dxValidator: requiredField"]> .dx-texteditor-container > .dx-texteditor-input').click()
    cy.wait(1000 * 1)
    cy.get('#txtClientLocation > .dx-texteditor-container > .dx-texteditor-input').clear().type('cypr6036')
    
    cy.get('.dx-popup-content > .get-to-work-orange > .dx-button-content').click()
    cy.get('div[data-bind="dxTextBox: txtUsername, dxValidator: requiredField"] > .dx-texteditor-container > .dx-texteditor-input').clear().type(user)
    cy.get('#accessPwd > .dx-texteditor-container > .dx-texteditor-input').clear().type(password)
    cy.get('div[data-bind="dxButton: btnLogin"]').click()

    cy.intercept('POST', 'https://vtportalidi.azurewebsites.net/api/portalsvcx.svc/Authenticate*').as('Authenticate');
    cy.intercept('GET', 'https://vtportalidi.azurewebsites.net/api/portalsvcx.svc/GetWsEmployeePhoto*').as('GetWsEmployeePhoto');
    cy.intercept('GET', 'https://vtportalidi.azurewebsites.net/api/portalsvcx.svc/GetMyPermissions*').as('GetMyPermissions');


    //cy.get("@Authenticate").its('response.statusCode').should('eq', 200);
    //cy.get("@GetWsEmployeePhoto").its('response.statusCode').should('eq', 200);
    //cy.get("@GetMyPermissions").its('response.statusCode').should('eq', 200);
    cy.wait(1000 * 3);

    return true
}
export function logoutPortal(): Promise<boolean> {


    cy.window().then((win) => {
        new win.WebServiceRobotics(function (result) {
            win.VTPortalUtils.utils.onLogoutSuccessFunc(result);
        }, function (error) {
            win.VTPortalUtils.utils.onLogoutErrorFunc(error);
        }).logout(win.VTPortal.roApp.db.settings.UUID);
    });
    return true
}