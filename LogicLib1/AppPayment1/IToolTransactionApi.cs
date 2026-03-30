using LogicLib1.AppModels1.Client;

namespace LogicLib1.AppPayment1;

public interface IToolTransactionApi
{
    Task<PaymongoQrphChargeRes> CreateQrphChargeAsyn(
         ClientRequest     req,
         CancellationToken ct = default);
}
