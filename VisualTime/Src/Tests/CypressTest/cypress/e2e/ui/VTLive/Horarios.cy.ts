import { method } from "cypress/types/lodash";
import * as loginHelper from "../../../helpers/loginHelper"


describe('Horarios Pantalla-', () => {

    beforeEach(() => {
        loginHelper.login('g.loco', '12345678As+')
        cy.intercept('POST', 'https://vtliveidi.azurewebsites.net/Shifts/ShiftSelectorData.aspx?OnlyGroups=0&ImagesPath=/Base/WebUserControls/../images/ShiftSelector&Filters=11110&FilterUserFields=&FilterFixed=&FeatureAlias=&FeatureType=U&ReloadGroups=false&MultiSelect=0').as('ShiftSelectorData');
        cy.window().then((win) => {
            win.reenviaFrame('/./Shifts/Shifts', '', '', '');
        });
        
        cy.wait('@ShiftSelectorData').its('response.statusCode').should('eq', 200);
    });

    afterEach(() => {
        loginHelper.logout()
    })



    it('Test para verificar que el usuario pueda ingresar al sistema y que los Iframes estén cargados accediendo a la sección de Horarios.', () => {


        cy.getIframeBody('#ifPrincipal').find("#ctl00_contentMainBody_roTreesShifts_spanCaption").invoke('text').then((text) => {
            expect(text.trim()).to.equal("Horarios".trim())
        })
    })

})
