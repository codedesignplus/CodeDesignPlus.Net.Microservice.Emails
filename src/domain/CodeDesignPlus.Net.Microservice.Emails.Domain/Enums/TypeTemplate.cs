namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;

public enum TypeTemplate
{
    None = 0,

    // Auth
    PasswordTemp = 1,
    Welcome = 2,
    PasswordReset = 3,
    AccountVerification = 4,
    TwoFactorCode = 5,

    // Compras/Licencias
    PurchaseConfirmation = 10,
    PurchaseReceipt = 11,
    PaymentFailed = 12,
    SubscriptionExpiring = 13,
    SubscriptionRenewed = 14,

    // Notificaciones
    InvitationToOrganization = 20,
    RoleChanged = 21,
    AccountDeactivated = 22,
    DataExportReady = 23
}