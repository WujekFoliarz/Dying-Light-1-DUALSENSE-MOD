cmake_minimum_required (VERSION 3.8)

# Enable Hot Reload for MSVC compilers if supported.
if (POLICY CMP0141)
  cmake_policy(SET CMP0141 NEW)
  set(CMAKE_MSVC_DEBUG_INFORMATION_FORMAT "$<IF:$<AND:$<C_COMPILER_ID:MSVC>,$<CXX_COMPILER_ID:MSVC>>,$<$<CONFIG:Debug,RelWithDebInfo>:EditAndContinue>,$<$<CONFIG:Debug,RelWithDebInfo>:ProgramDatabase>>")
endif()

project (MyProject)
file(GLOB_RECURSE MY_SOURCES CONFIGURE_DEPENDS "${CMAKE_CURRENT_SOURCE_DIR}/source/*.cpp")

set(COMPILE_TO_DLL OFF CACHE BOOL "Compile to dll" FORCE) # Delete the out folder when you change this

if(COMPILE_TO_DLL)
add_library(${PROJECT_NAME} SHARED ${MY_SOURCES})
target_compile_definitions(${PROJECT_NAME} PUBLIC COMPILE_TO_DLL=1)
else()
add_executable(${PROJECT_NAME} ${MY_SOURCES})
target_compile_definitions(${PROJECT_NAME} PUBLIC COMPILE_TO_DLL=0)
endif()

set_target_properties(${PROJECT_NAME} PROPERTIES POSITION_INDEPENDENT_CODE ON)
target_include_directories(${PROJECT_NAME}
  PUBLIC
    "${CMAKE_CURRENT_SOURCE_DIR}/include"
    "${CMAKE_CURRENT_SOURCE_DIR}/source"
)

# Subdirectiories
# add_subdirectory(thirdparty/)

# Linking libraries
# target_link_libraries(${PROJECT_NAME} PRIVATE )

# Copying DLLs to out folder
# add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy_if_different $<TARGET_FILE:YourTarget> $<TARGET_FILE_DIR:${PROJECT_NAME}>)