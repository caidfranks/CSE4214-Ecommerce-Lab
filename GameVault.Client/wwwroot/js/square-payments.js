let payments;
let card;
let isInitialized = false;

window.initializeSquarePayments = async function (dotNetHelperRef) {
    if (isInitialized) {
        console.log('Square payments already initialized');
        return;
    }

    try {
        const container = document.getElementById('card-container');
        if (!container) {
            throw new Error('card-container element not found in DOM');
        }

        const applicationId = 'sandbox-sq0idb-UqNPJw5lxgVgk9XIXeI3nw';
        const locationId = 'L3VRG5E7HM55C';

        payments = window.Square.payments(applicationId, locationId);
        card = await payments.card();
        await card.attach('#card-container');

        isInitialized = true;
        console.log('Square Payments initialized successfully');
    } catch (error) {
        console.error('Error initializing Square Payments:', error);
        if (dotNetHelperRef) {
            dotNetHelperRef.invokeMethodAsync('HandlePaymentError', 'Failed to initialize payment form: ' + error.message);
        }
        throw error;
    }
};

async function handlePaymentMethodSubmission(dotNetHelper) {
    try {
        if (!card) {
            throw new Error('Payment form not initialized');
        }

        if (!dotNetHelper) {
            throw new Error('DotNet helper reference not provided');
        }

        const result = await card.tokenize();

        if (result.status === 'OK') {
            await dotNetHelper.invokeMethodAsync('HandlePaymentMethodCreated', result.token);
        } else {
            let errorMessage = 'Payment processing failed';
            if (result.errors) {
                errorMessage = result.errors.map(error => error.message).join(', ');
            }
            dotNetHelper.invokeMethodAsync('HandlePaymentError', errorMessage);
        }
    } catch (error) {
        console.error('Error during payment submission:', error);
        if (dotNetHelper) {
            dotNetHelper.invokeMethodAsync('HandlePaymentError', 'An unexpected error occurred: ' + error.message);
        }
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
    isInitialized = false;
};