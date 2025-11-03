let payments;
let card;

window.initializeSquarePayments = async function (dotNetHelper) {
    try {
        const applicationId = 'sandbox-sq0idb-UqNPJw5lxgVgk9XIXeI3nw';
        const locationId = 'L3VRG5E7HM55C';

        payments = window.Square.payments(applicationId, locationId);

        card = await payments.card();
        await card.attach('#card-container');

        console.log('Square Payments initialized successfully');

        card.addEventListener('submit', async (event) => {
            event.preventDefault();
            await handlePaymentMethodSubmission(dotNetHelper);
        });

    } catch (error) {
        console.error('Error initializing Square Payments:', error);
        dotNetHelper.invokeMethodAsync('HandlePaymentError', 'Failed to initialize payment form');
    }
};

async function handlePaymentMethodSubmission(dotNetHelper) {
    try {
        const submitButton = document.querySelector('button[type="submit"]');
        if (submitButton) {
            submitButton.disabled = true;
        }

        const result = await card.tokenize();

        if (result.status === 'OK') {
            console.log('Payment token created:', result.token);
            
            await dotNetHelper.invokeMethodAsync('HandlePaymentMethodCreated', result.token);
        } else {

            let errorMessage = 'Payment processing failed';
            
            if (result.errors) {
                errorMessage = result.errors.map(error => error.message).join(', ');
            }
            
            console.error('Tokenization errors:', result.errors);
            dotNetHelper.invokeMethodAsync('HandlePaymentError', errorMessage);
        }
    } catch (error) {
        console.error('Error during payment method submission:', error);
        dotNetHelper.invokeMethodAsync('HandlePaymentError', 'An unexpected error occurred');
    }
}

window.tokenizeSquareCard = async function (dotNetHelper) {
    await handlePaymentMethodSubmission(dotNetHelper);
};

window.destroySquarePayments = function () {
    if (card) {
        card.destroy();
        card = null;
    }
    payments = null;
};