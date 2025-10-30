import * as baseHelper from "../../../helpers/baseHelper"
import * as loginHelper from "../../../helpers/loginHelper"



describe('Portal-', () => {

  afterEach(() => {
    loginHelper.logoutPortal()
  }),



    it('Pantalla de login con login positivo', async () => {


      cy.intercept('POST', 'https://vtportalidi.azurewebsites.net/api/portalsvcx.svc/Authenticate').as('Authenticate');

      cy.clearAllCookies();
      //cy.clearAllSessionStorage();
     // cy.clearAllLocalStorage();
      cy.visit("https://vtportalidi.azurewebsites.net/2/indexv2.aspx")
      cy.wait(1000*5)





    // Intenta obtener el elemento y hacer clic en él si existe
 
      if (cy.find('.dx-popup-bottom > .dx-toolbar-items-container > .dx-toolbar-after > .dx-item > .dx-item-content')) {
        cy.get('.dx-popup-bottom > .dx-toolbar-items-container > .dx-toolbar-after > .dx-item > .dx-item-content').click();
      } else {
        cy.log('El elemento no existe');
      }





      cy.get(':nth-child(1) > .dx-field-value > .dx-texteditor-container > .dx-texteditor-input').click()
      cy.get('#txtClientLocation > .dx-texteditor-container > .dx-texteditor-input').clear().type('cypr6036')
      cy.get('.dx-popup-content > .get-to-work-orange > .dx-button-content').click()
      cy.wait(1000*3)
      cy.get(':nth-child(2) > .dx-field-value > .dx-texteditor-container > .dx-texteditor-input').clear().type('g.loco')
      cy.get('#accessPwd > .dx-texteditor-container > .dx-texteditor-input').clear().type('12345678As+')

      cy.wait(1000*3)
      cy.get('.dx-scrollview-content > .get-to-work-orange > .dx-button-content').click()
      cy.wait(1000*3)

      cy.get("@Authenticate").its('response.statusCode').should('eq', 200);


     cy.get("@Authenticate").its('response.body').then((body) => {
     // json = JSON.parse(body)

      cy.log(body['d']['UserId'])
      //expect(json).to.have.property('IsAuthenticated', true)
      //expect(json).to.have.property('UserId', true)
      expect(body['d']['UserId']).to.eq(161);
    })
    
    })
})

