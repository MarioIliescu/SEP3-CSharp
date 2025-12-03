using Microsoft.AspNetCore.Mvc;
using Services.RecruitDriver;

namespace FleetWebApi.Controllers;


[ApiController]
[Route("RecruitDriver")]
public class RecruitDriverController : ControllerBase
{
    private readonly IRecruitDriverService _recruitDriverService;
    
    public RecruitDriverController(IRecruitDriverService recruitDriverService)
    {
        _recruitDriverService = recruitDriverService;
    }
    
}