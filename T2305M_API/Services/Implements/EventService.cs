using AutoMapper;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using T2305M_API.DTO.Culture;
using T2305M_API.DTO.Event;
using T2305M_API.DTO.History;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Repositories;
using T2305M_API.Repositories.Implements;
using T2305M_API.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ICreatorRepository _creatorRepository;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;
    private readonly T2305mApiContext _context;


    public EventService(IEventRepository eventRepository, IWebHostEnvironment env, ICreatorRepository creatorRepository, IMapper mapper, T2305mApiContext context)
    {
        _eventRepository = eventRepository;
        _env = env;
        _creatorRepository = creatorRepository;
        _mapper = mapper;
        _context = context;

    }

    public async Task<PaginatedResult<GetBasicEventDTO>> GetBasicEventDTOsAsync(EventQueryParameters queryParameters)
    {
        var (data, totalItems) = await _eventRepository.GetEventsAsync(queryParameters);

        var basicEventDTOs = new List<GetBasicEventDTO>();

        foreach (var a in data)
        {
            var getBasicEventDTO = _mapper.Map<GetBasicEventDTO>(a);
            basicEventDTOs.Add(getBasicEventDTO);
        }
        // Calculate total pages
        int totalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize);

        return new PaginatedResult<GetBasicEventDTO>
        {
            TotalItems = totalItems,
            PageSize = queryParameters.PageSize,
            CurrentPage = queryParameters.Page,
            TotalPages = totalPages,
            HasNextPage = queryParameters.Page < totalPages,
            HasPreviousPage = queryParameters.Page > 1,
            Data = basicEventDTOs
        };
    }
    public async Task<GetDetailEventDTO> GetDetailEventDTOByIdAsync(int eventId)
    {
        var eventInstance = await _eventRepository.GetEventByIdAsync(eventId);

        if (eventInstance == null)
        {
            return null; // Or throw an appropriate exception
        }


        var detailEventDTO = _mapper.Map<GetDetailEventDTO>(eventInstance);

        return detailEventDTO;
    }
    public async Task<CreateEventResponseDTO> CreateEventAsync(CreateEventDTO createEventDTO)
    {
        CreateEventResponseDTO createEventResponseDTO = await _eventRepository.CreateEventAsync(createEventDTO);
        var newImagePath = await SaveImageAsync(createEventDTO.formFile);
        Event eventEntity = await _eventRepository.GetEventByIdAsync(createEventResponseDTO.EventId);
        // Update the event entity with the new image path (or any other modifications)
        eventEntity.Thumbnail = newImagePath;
        // Update the event in the database
        _context.Event.Update(eventEntity);
        await _context.SaveChangesAsync();
        return createEventResponseDTO;
    }
    public async Task<Dictionary<string, List<string>>> ValidateCreateEventDTO(CreateEventDTO createEventDTO)
    {

        var errors = new Dictionary<string, List<string>>();

        // Validate file size (example: limit to 5 MB)
        const long maxFileSize = 5 * 1024 * 1024;  // 5 MB
        var image = createEventDTO.formFile;
        if (image.Length > maxFileSize)
        {
            AddError(errors, "Thumbnail", "File size cannot exceed 5 MB.");
        }

        // Validate file type by checking MIME type and extension
        var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
        {
            AddError(errors, "Thumbnail", "Invalid file extension. Only .jpg, .jpeg, .png are allowed.");
        }

        if (!image.ContentType.StartsWith("image/"))
        {
            AddError(errors, "Thumbnail", "Only image files are allowed.");
        }


        return errors.Count > 0 ? errors : null;
    }
    private static void AddError(Dictionary<string, List<string>> errors, string key, string errorMessage)
    {
        if (!errors.ContainsKey(key))
        {
            errors[key] = new List<string>();
        }
        errors[key].Add(errorMessage);
    }
    private async Task<string> SaveImageAsync(IFormFile image)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/images/userArticleThumbnails");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return $"/uploads/images/userArticleThumbnails/{uniqueFileName}";
    }

    public async Task<UpdateEventResponseDTO> UpdateEventAsync(UpdateEventDTO updateEventDTO)
    {
        UpdateEventResponseDTO updateEventResponseDTO = await _eventRepository.UpdateEventAsync(updateEventDTO);
        var newImagePath = await SaveImageAsync(updateEventDTO.formFile);
        Event eventEntity = await _eventRepository.GetEventByIdAsync(updateEventResponseDTO.EventId);
        // Update the event entity with the new image path (or any other modifications)
        eventEntity.Thumbnail = newImagePath;
        // Update the event in the database
        _context.Event.Update(eventEntity);
        await _context.SaveChangesAsync();
        return updateEventResponseDTO;
    }

}