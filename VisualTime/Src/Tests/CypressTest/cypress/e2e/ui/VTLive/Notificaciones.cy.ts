import { method } from "cypress/types/lodash";
import * as loginHelper from "../../../helpers/loginHelper"


describe('Notificaciones Pantalla-', () => {

    beforeEach(() => {
        loginHelper.login('g.loco', '12345678As+')
        cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Notifications/NotificationSelectorData.aspx?OnlyGroups=0&ImagesPath=/Base/WebUserControls/../images/NotificationSelector&Filters=11110&FilterUserFields=&FilterFixed=&FeatureAlias=&FeatureType=U&ReloadGroups=false&MultiSelect=0').as('NotificationSelectorData');

        cy.window().then((win) => {
            win.reenviaFrame('/./Notifications/Notifications', '', '', '');
        });
 
        cy.wait('@NotificationSelectorData').its('response.statusCode').should('eq', 200);
    });

    afterEach(() => {
        loginHelper.logout()
    })



    it('Test para verificar que el usuario pueda ingresar al sistema y que los Iframes estén cargados accediendo a la sección de Notificaciones.', () => {

       
        cy.getIframeBody('#ifPrincipal').find("#ctl00_contentMainBody_roTreesNotifications_spanCaption").invoke('text').then((text) => {
            expect(text.trim()).to.equal("Notificaciones".trim())
        })
    })

})